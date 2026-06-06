using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class TransferFraudService : ITransferFraudService
    {
        private readonly ILogger<TransferFraudService> _logger;

        public TransferFraudService(ILogger<TransferFraudService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(TransferCompletedEvent transferCompletedEvent)
        {
            if(transferCompletedEvent.Amount >= 1000000)
            {
                _logger.LogWarning("Fraud alert for {Reference}", transferCompletedEvent.Reference);
            }
            return Task.CompletedTask;
        }
    }
}
