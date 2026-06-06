using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class DepositNotificationService : IDepositNotificationService
    {
        private readonly ILogger<DepositNotificationService> _logger;

        public DepositNotificationService(ILogger<DepositNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(DepositCompletedEvent depositEvent)
        {
            _logger.LogInformation("Notification sent for deposit {Reference}", depositEvent.Reference);
            return Task.CompletedTask;
        }
    }
}
