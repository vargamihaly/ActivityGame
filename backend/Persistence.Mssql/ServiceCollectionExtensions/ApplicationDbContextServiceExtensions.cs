using ActivityGameBackend.Persistence.Mssql.Games;
using ActivityGameBackend.Common.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ActivityGameBackend.Persistence.Mssql.ServiceCollectionExtensions;
public static class AppDbContextServiceExtensions
{
    public static IServiceCollection AddMsSqlAppDbContext(this IServiceCollection services,
        IConfiguration configuration, string? environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (environment == HostingEnvironments.Production)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(builder.ConnectionString));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(builder.ConnectionString).EnableSensitiveDataLogging());
        }

        return services;
    }

    public static IServiceCollection AddMsSqlDbServiceProvider(this IServiceCollection services)
    {
        services.AddScoped<IGameServiceDataProvider, GameServiceDataProvider>();
        return services;
    }
}

