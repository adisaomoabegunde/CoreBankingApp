using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class WithdrawalNotificationService : IWithdrawalNotificationService
    {
        private readonly ILogger<WithdrawalNotificationService> _logger;

        public WithdrawalNotificationService(ILogger<WithdrawalNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(WithdrawalCompletedEvent withdrawalEvent)
        {
            _logger.LogInformation("Withdrawal alert sent for {Reference}", withdrawalEvent.Reference);
            return Task.CompletedTask;
        }
    }
}
