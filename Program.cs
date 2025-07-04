using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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

    // ✅ CORS: Safe dynamic origin handling
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "OnlyFrontend",
            policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => Uri.TryCreate(origin, UriKind.Absolute, out _))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // required for cookie-based auth, optional for JWT
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
        c.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            }
        );
        c.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            }
        );
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

    // ✅ JWT Configuration
    var jwtKey = configuration["Jwt:Key"];
    var jwtIssuer = configuration["Jwt:Issuer"];
    var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

    builder
        .Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // turn true in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
            };
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

    app.UseAuthentication(); // JWT is in play here
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
