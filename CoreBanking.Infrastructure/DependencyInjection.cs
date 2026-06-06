using AutoMapper;
using CoreBanking.Application.Interfaces;
using CoreBanking.Infrastructure.Messaging;
using CoreBanking.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
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
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();
            services.AddSingleton<IEventProducer, KafkaProducer>();

            return services;
        }
    }
}
