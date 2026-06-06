using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class CustomerAuditService : ICustomerAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<CustomerAuditService> _logger;


        public CustomerAuditService(IAuditRepository auditRepository, ILogger<CustomerAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }


        public async Task HandleAsync(CustomerRegisteredEvent registeredEvent)
        {
            try
            {
                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "CustomerRegistered",
                    Description = $"Customer {registeredEvent.Email} registered",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = registeredEvent.CustomerId
                };

                await _auditRepository.AddAsync(audit);

                _logger.LogInformation("Audit log created for customer: {CustomerId}", registeredEvent.CustomerId);
            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Customer registration failed",
                    Description = $"Customer {registeredEvent.Email} registration failed",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = registeredEvent.CustomerId
                });

                _logger.LogError(ex, "Failed to create Audit log for customer: {CustomerId}", registeredEvent.CustomerId);
            }
        }

    }
}
