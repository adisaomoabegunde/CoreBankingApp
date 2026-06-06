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
    public class DepositAuditService : IDepositAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<DepositAuditService> _logger;

        public DepositAuditService(IAuditRepository auditRepository, ILogger<DepositAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(DepositCompletedEvent depositEvent)
        {
            try
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "DepositCompleted",
                    Description = $"Deposit of {depositEvent.Amount} to account {depositEvent.AccountId} completed.",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = Guid.Empty
                });
                _logger.LogInformation("Audit log created for {Reference}", depositEvent.Reference);

            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "DepositFailed",
                    Description = $"Deposit of {depositEvent.Amount} to account {depositEvent.AccountId} failed.",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = Guid.Empty
                });

                _logger.LogError(ex, "Failed to create audit log for {Reference}", depositEvent.Reference);

            }
        }
    }
}
