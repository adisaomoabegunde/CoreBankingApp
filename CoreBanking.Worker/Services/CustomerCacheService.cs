using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class CustomerCacheService : ICustomerCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<CustomerCacheService> _logger;

        public CustomerCacheService(IConnectionMultiplexer redis, ILogger<CustomerCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }


        public async Task HandleAsync(CustomerRegisteredEvent registeredEvent)
        {
            var key = $"customer:{registeredEvent.CustomerId}";

            var value = JsonSerializer.Serialize(new
            {
                registeredEvent.CustomerId,
                registeredEvent.Email,
                registeredEvent.FirstName,
                registeredEvent.LastName,
            });
            await _db.StringSetAsync(key, value);

            _logger.LogInformation("Customer cached: {CustomerId}", registeredEvent.CustomerId);
        }

    }
}
