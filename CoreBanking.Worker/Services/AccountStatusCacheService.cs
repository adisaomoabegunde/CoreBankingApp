using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class AccountStatusCacheService : IAccountStatusCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<AccountStatusCacheService> _logger;

        public AccountStatusCacheService(IConnectionMultiplexer redis, ILogger<AccountStatusCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(AccountStatusChangedEvent accountStatusChanged)
        {
            var key = $"account:{accountStatusChanged.AccountId}:status";
            await _db.StringSetAsync(key, accountStatusChanged.NewStatus.ToString());

            _logger.LogInformation("Redis cache updated for account {AccountNumber} status change to {NewStatus}", accountStatusChanged.AccountNumber, accountStatusChanged.NewStatus);
        }
    }
}
