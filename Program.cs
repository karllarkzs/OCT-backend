using Microsoft.OpenApi.Models;
using PharmaBack;
using PharmaBack.Data;
using PharmaBack.Helpers;
using PharmaBack.Services.Catalog;
using PharmaBack.Services.Crud;
using PharmaBack.Services.Products;

try
{
    var builder = WebApplication.CreateBuilder(args);
    if (!builder.Environment.IsDevelopment())
    {
        ElevationHelper.EnsureElevated();
        StaticIpManager.TryAssignStatic(5);
    }
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "PharmaBack API", Version = "v1" });
    });
    builder.Services.AddControllers();

    builder.Services.AddPharmaDatabase(builder.Configuration);
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICatalogQuery, CatalogQuery>();
    builder.Services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));

    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStopping.Register(() =>
    {
        StaticIpManager.RestoreDhcp();
    });

    app.Run();
}
finally
{
    StaticIpManager.RestoreDhcp();
}
