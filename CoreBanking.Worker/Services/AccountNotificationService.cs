using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class AccountNotificationService : IAccountNotificationService
    {
        private readonly ILogger<AccountNotificationService> _logger;

        public AccountNotificationService(ILogger<AccountNotificationService> logger)
        {
            _logger = logger;
        }


        public Task HandleAsync(AccountCreatedEvent accountCreatedEvent)
        {
            _logger.LogInformation("Notification sent for account {AccountNumber} creation", accountCreatedEvent.AccountNumber);
            return Task.CompletedTask;


        }
    }
}
