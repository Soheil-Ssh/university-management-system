using Identity.Api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the sql server connection string from the configuration
        var sqlServerConnectionString = configuration.GetConnectionString("SqlServerDefaultConnection");

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<IdentityDbContext>(options 
            => options.UseSqlServer(sqlServerConnectionString));

        return services;
    }
}