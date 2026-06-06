using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class AccountStatusAuditService : IAccountStatusAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<AccountStatusAuditService> _logger;

        public AccountStatusAuditService(IAuditRepository auditRepository, ILogger<AccountStatusAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(AccountStatusChangedEvent accountStatusChangedEvent)
        {
            try
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "AccountStatusChanged",
                    Description = $"Account status changed from {accountStatusChangedEvent.OldStatus} to {accountStatusChangedEvent.NewStatus}",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = Guid.Empty
                });

                _logger.LogInformation("Audit Log created for account status change for {AccountNumber}", accountStatusChangedEvent.AccountNumber);
            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Acoount status change failed",
                    Description = $"Account status change failed for account {accountStatusChangedEvent.AccountNumber}",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = Guid.Empty
                });

                _logger.LogError(ex, "Failed to change account status for {AccountNumber}", accountStatusChangedEvent.AccountNumber);
            }
        }
    }
}
