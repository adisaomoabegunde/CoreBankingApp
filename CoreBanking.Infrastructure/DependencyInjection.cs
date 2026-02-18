using CoreBanking.Application.Interfaces;
using CoreBanking.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

            return services;
        }
    }
}
