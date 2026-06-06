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
    public class AccountAuditService : IAccountAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<AccountAuditService> _logger;

        public AccountAuditService(IAuditRepository auditRepository, ILogger<AccountAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(AccountCreatedEvent accountCreatedEvent)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Account Created",
                    Description = $"Account {accountCreatedEvent.AccountNumber} created for customer {accountCreatedEvent.CustomerId}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System",
                    UserId = Guid.Empty
                };

                await _auditRepository.AddAsync(auditLog);
                _logger.LogInformation("Audit log created for account {AccountNumber} creation", accountCreatedEvent.AccountNumber);
            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Account Creation Failed",
                    Description = $"Failed to create audit log for account {accountCreatedEvent.AccountNumber}: {ex.Message}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System",
                    UserId = Guid.Empty
                });
                _logger.LogError(ex, "Failed to create audit log for account {AccountNumber} creation", accountCreatedEvent.AccountNumber);
            }
        }    
    }
}
