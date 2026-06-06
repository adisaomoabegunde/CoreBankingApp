using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class DepositFraudService : IDepositFraudService
    {
        private readonly ILogger<DepositFraudService> _logger;

        public DepositFraudService(ILogger<DepositFraudService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(DepositCompletedEvent depositEvent)
        {
            if(depositEvent.Amount > 1000000)
            {
                _logger.LogWarning("Large deposit flagged: {Reference}", depositEvent.Reference);
            }

            return Task.CompletedTask;
        }
    }
}
