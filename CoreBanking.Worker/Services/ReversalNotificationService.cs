using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class ReversalNotificationService : IReversalNotificationService
    {
        private readonly ILogger<ReversalNotificationService> _logger;

        public ReversalNotificationService(ILogger<ReversalNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(TransactionReversedEvent reversedEvent)
        {
            _logger.LogInformation("Reversal notification sent for {Reference}", reversedEvent.Reference);
            return Task.CompletedTask;
        }
    }
}
