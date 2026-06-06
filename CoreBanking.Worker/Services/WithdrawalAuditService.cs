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
    public class WithdrawalAuditService : IWithdrawalAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<WithdrawalAuditService> _logger;

        public WithdrawalAuditService(IAuditRepository auditRepository, ILogger<WithdrawalAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(WithdrawalCompletedEvent withdrawalEvent)
        {
            try
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "WithdrawalCompleted",
                    Description = $"Withdrawal of {withdrawalEvent.Amount} from account {withdrawalEvent.AccountId} completed.",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System",
                    UserId = Guid.Empty
                });
                _logger.LogInformation("Audit log created for {Reference}", withdrawalEvent.Reference);
            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "WithdrawalFailed",
                    Description = $"Withdrawal of {withdrawalEvent.Amount} from account {withdrawalEvent.AccountId} failed.",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System",
                    UserId = Guid.Empty
                });
                _logger.LogError(ex, "Failed to create audit log for {Reference}", withdrawalEvent.Reference);

            }
        }
    }
}
