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
    public class CustomerKycCacheService : ICustomerKycCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<CustomerKycCacheService> _logger;

        public CustomerKycCacheService(IConnectionMultiplexer redis, ILogger<CustomerKycCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(CustomerKycUpdatedEvent kycUpdatedEvent)
        {
            var key = $"customer:{kycUpdatedEvent.CustomerId}";

            var existing = await _db.StringGetAsync(key);

            if (existing.HasValue)
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(existing);

                data["KycStatus"] = kycUpdatedEvent.KycStatus;

                await _db.StringSetAsync(key, JsonSerializer.Serialize(data));
            }

            _logger.LogInformation("Customer KYC cache updated: {CustomerId}", kycUpdatedEvent.CustomerId);
        }
    }
}
