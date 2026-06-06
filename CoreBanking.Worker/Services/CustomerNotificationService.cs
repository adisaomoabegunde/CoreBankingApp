using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class CustomerNotificationService : ICustomerNotificationService
    {
        private readonly ILogger<CustomerNotificationService> _logger;

        public CustomerNotificationService(ILogger<CustomerNotificationService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(CustomerRegisteredEvent customerRegisteredEvent)
        {
            _logger.LogInformation("Notification sent to {Email} for customer registration", customerRegisteredEvent.Email);
            return Task.CompletedTask;
        }
    }
}
