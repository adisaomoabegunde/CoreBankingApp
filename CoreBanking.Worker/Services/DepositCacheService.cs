using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace CoreBanking.Worker.Services
{
    public class DepositCacheService : IDepositCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<DepositCacheService> _logger;

        public DepositCacheService(IConnectionMultiplexer redis, ILogger<DepositCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(DepositCompletedEvent depositEvent)
        {
            var key = $"account:{depositEvent.AccountId}:balance";

            await _db.StringIncrementAsync(key, (double)depositEvent.Amount);

            _logger.LogInformation("Updated cache for account {AccountId} with new balance after deposit of {Amount}", depositEvent.AccountId, depositEvent.Amount);

        }
    }
}
