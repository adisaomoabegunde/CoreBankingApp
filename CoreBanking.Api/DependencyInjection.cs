using CoreBanking.Application;
using CoreBanking.Infrastructure;

namespace CoreBanking.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {

            services.AddApplicationDI()
                .AddInfrastructureDI();

            return services;
        }
    }
}
