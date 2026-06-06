using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class CustomerKycNotificationService : ICustomerKycnotificationService
    {
        private readonly ILogger<CustomerKycNotificationService> _logger;

        public CustomerKycNotificationService(ILogger<CustomerKycNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(CustomerKycUpdatedEvent customerKycUpdatedEvent)
        {
            _logger.LogInformation("KYC status updated to {Status} for CustomerId: {CustomerId}", customerKycUpdatedEvent.KycStatus, customerKycUpdatedEvent.CustomerId);
            return Task.CompletedTask;
        }
    }
}
