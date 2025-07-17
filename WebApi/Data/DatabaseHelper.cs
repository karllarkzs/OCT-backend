using Microsoft.EntityFrameworkCore;
using Npgsql;
using PharmaBack.WebApi.Data;

namespace PharmaBack.WebApi.Data
{
    public static class DatabaseHelper
    {
        public static IServiceCollection AddPharmaDatabase(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = GetConnectionString(configuration);

            services.AddDbContext<PharmaDbContext>(options => options.UseNpgsql(connectionString));

            return services;
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var host = configuration["Database:Host"];
            var port = configuration["Database:Port"];
            var database = configuration["Database:Database"];
            var username = configuration["Database:Username"];
            var password = configuration["Database:Password"];

            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Prefer";
        }
    }
}
