using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class TransferNotificationService : ITransferNotificationService
    {
        private readonly ILogger<TransferNotificationService> _logger;

        public TransferNotificationService(ILogger<TransferNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(TransferCompletedEvent transferCompletedEvent)
        {
           _logger.LogInformation("Notification sent for {Reference}", transferCompletedEvent.Reference);
            return Task.CompletedTask;
        }
    }
}
