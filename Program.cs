using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using PharmaBack;
using PharmaBack.Data;
using PharmaBack.Helpers;
using PharmaBack.Models;
using PharmaBack.Services.Auth;
using PharmaBack.Services.Bundles;
using PharmaBack.Services.Catalogs;
using PharmaBack.Services.Crud;
using PharmaBack.Services.Locations;
using PharmaBack.Services.Products;
using PharmaBack.Services.Transactions;

static async Task SeedRolesAndAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    string[] roles = ["cashier", "admin"];
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

try
{
    var configBuilder = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    var configuration = configBuilder.Build();

    var builder = WebApplication.CreateBuilder(
        new WebApplicationOptions { Args = args, ContentRootPath = AppContext.BaseDirectory }
    );
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5050);
    });

    builder.Configuration.AddConfiguration(configuration);
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "OnlyFrontend",
            policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }
        );
    });

    if (!builder.Environment.IsDevelopment())
    {
        ElevationHelper.EnsureElevated();
        StaticIpManager.TryAssignStatic(5);
    }

    _ = typeof(JsonSerializer).Assembly;

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "PharmaBack API", Version = "v1" });
    });
    builder
        .Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            );
        });
    builder.Services.AddPharmaDatabase(builder.Configuration);
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IBundleService, BundleService>();
    builder.Services.AddScoped<ICatalogQuery, CatalogQuery>();
    builder.Services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
    builder.Services.AddScoped<ILocationService, LocationService>();
    builder.Services.AddScoped<ITransactionService, TransactionService>();

    builder
        .Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<PharmaDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.Name = "PharmaAuth";
        options.LoginPath = "/api/auth/signin";
        options.LogoutPath = "/api/auth/signout";
        options.AccessDeniedPath = "/api/auth/forbidden";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder
        .Services.AddAuthorizationBuilder()
        .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
        .AddPolicy("CashierOnly", policy => policy.RequireRole("cashier"));
    builder.Services.AddAuthorization();

    var app = builder.Build();
    app.UseCors("OnlyFrontend");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStopping.Register(() =>
    {
        StaticIpManager.RestoreDhcp();
    });
    await SeedRolesAndAdminAsync(app.Services);
    app.Run();
}
finally
{
    StaticIpManager.RestoreDhcp();
}
