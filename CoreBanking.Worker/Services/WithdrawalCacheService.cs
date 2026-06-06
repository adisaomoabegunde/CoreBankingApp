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
    public class WithdrawalCacheService : IWithdrawalCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<WithdrawalCacheService> _logger;

        public WithdrawalCacheService(IConnectionMultiplexer redis, ILogger<WithdrawalCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(WithdrawalCompletedEvent withdrawalEvent)
        {
            var accountKey = $"account:{withdrawalEvent.AccountId}:balance";

            await _db.StringDecrementAsync(accountKey, (double)withdrawalEvent.Amount);

            _logger.LogInformation("Redis updated for withdrawal {Reference}", withdrawalEvent.Reference);
        }
    }
}
