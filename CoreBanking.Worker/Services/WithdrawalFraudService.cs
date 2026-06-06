using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class WithdrawalFraudService : IWithdrawalFraudService
    {
        private readonly ILogger<WithdrawalFraudService> _logger;

        public WithdrawalFraudService(ILogger<WithdrawalFraudService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(WithdrawalCompletedEvent withdrawalEvent)
        {
            if (withdrawalEvent.Amount > 100000) // Simple fraud check
            {
                _logger.LogWarning($"Potential fraud detected for withdrawal of {withdrawalEvent.Amount} from account {withdrawalEvent.AccountId}");
            }
            return Task.CompletedTask;

        }
    }
}
