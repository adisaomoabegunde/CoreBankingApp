using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class AccountStatusNotificationService : IAccountStatusNotificationService
    {
        private readonly ILogger<AccountStatusNotificationService> _logger;

        public AccountStatusNotificationService(ILogger<AccountStatusNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(AccountStatusChangedEvent accountStatusChangedEvent)
        {
            _logger.LogInformation("Notification sent for account {AccountNumber} status change to {NewStatus}", accountStatusChangedEvent.AccountNumber, accountStatusChangedEvent.NewStatus);
            return Task.CompletedTask;
        }
    }
}
