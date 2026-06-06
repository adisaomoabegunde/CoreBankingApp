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
    public class AccountCacheService : IAccountCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<AccountCacheService> _logger;

        public AccountCacheService(IConnectionMultiplexer redis, ILogger<AccountCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(AccountCreatedEvent accountCreatedEvent)
        {
            var accountKey = $"account:{accountCreatedEvent.AccountId}:balance";
            await _db.StringSetAsync(accountKey, (double)accountCreatedEvent.InitialBalance);
            _logger.LogInformation("Redis cache initialized for account {AccountNumber}", accountCreatedEvent.AccountNumber);
        }
    }
}
