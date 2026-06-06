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
    public class ReversalCacheService : IReversalCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<ReversalCacheService> _logger;

        public ReversalCacheService(IConnectionMultiplexer redis, ILogger<ReversalCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(TransactionReversedEvent reversedEvent)
        {
            var accountKey = $"account:{reversedEvent.AccountId}:balance";

            await _db.StringIncrementAsync(accountKey,(double)reversedEvent.Amount);
            _logger.LogInformation($"Updated cache for AccountId: {reversedEvent.AccountId}, Amount: {reversedEvent.Amount}, New Balance: {await _db.StringGetAsync(accountKey)}");
        }
    }
}
