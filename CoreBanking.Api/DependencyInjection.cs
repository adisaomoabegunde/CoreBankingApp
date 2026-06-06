using CoreBanking.Application;
using CoreBanking.Infrastructure;

// Core
namespace CoreBanking.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddApplicationDI()
                .AddInfrastructureDI(configuration);

            return services;
        }
    }
}
