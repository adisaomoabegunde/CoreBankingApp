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
    public class TransferCacheService : ITransferCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<TransferCacheService> _logger;

        public TransferCacheService(IConnectionMultiplexer redis, ILogger<TransferCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task HandleAsync(TransferCompletedEvent transferCompletedEvent)
        {
            var sourceKey = $"account:{transferCompletedEvent.SourceAccountId}:balance";
            var destinationKey = $"account:{transferCompletedEvent.DestinationAccountId}:balance";

            await _db.StringDecrementAsync(sourceKey, (double)transferCompletedEvent.Amount);
            await _db.StringIncrementAsync(destinationKey, (double)transferCompletedEvent.Amount);

            _logger.LogInformation("Redis updated for {Reference}", transferCompletedEvent.Reference);
        }
    }
}
