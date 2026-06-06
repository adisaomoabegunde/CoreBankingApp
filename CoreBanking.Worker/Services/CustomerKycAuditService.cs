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
    public class CustomerKycAuditService : ICustomerKycAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<CustomerKycAuditService> _logger;

        public CustomerKycAuditService( IAuditRepository auditRepository, ILogger<CustomerKycAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
          
        }

        public async Task HandleAsync(CustomerKycUpdatedEvent kycUpdatedEvent)
        {
            try
            {
                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "CustomerKycUpdated",
                    Description = $"KYC updated to {kycUpdatedEvent.KycStatus} for Customer {kycUpdatedEvent.CustomerId}",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = kycUpdatedEvent.CustomerId
                };
                await _auditRepository.AddAsync(audit);
                _logger.LogInformation("KYC audit log created for customer: {CustomerId}", kycUpdatedEvent.CustomerId);
            }catch(Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Customer Kyc Update failed",
                    Description = $"KYC Updated failed for Customer {kycUpdatedEvent.CustomerId}",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = kycUpdatedEvent.CustomerId
                });

                _logger.LogError(ex, "KYC audit log failed for customer: {CustomerId}", kycUpdatedEvent.CustomerId);
            }

        }
    }
}
