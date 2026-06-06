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
    public class TransferAuditService : ITransferAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<TransferAuditService> _logger;

        public TransferAuditService(IAuditRepository auditRepository, ILogger<TransferAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(TransferCompletedEvent transferCompletedEvent)
        {
            try
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "TransferCompleted",
                    Description = $"Traansfer {transferCompletedEvent.Reference} completed, Transferred {transferCompletedEvent.Amount} from {transferCompletedEvent.SourceAccountId} to {transferCompletedEvent.DestinationAccountId} completed",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System"

                });

                _logger.LogInformation("Audit log created for {Reference}", transferCompletedEvent.Reference);
            } catch(Exception ex)
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "TransferFailed",
                    Description = $"Traansfer {transferCompletedEvent.Reference} failed, Transferred {transferCompletedEvent.Amount} from {transferCompletedEvent.SourceAccountId} to {transferCompletedEvent.DestinationAccountId} failed",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System"

                });
                _logger.LogError(ex, "Failed to create audit log for {Reference}", transferCompletedEvent.Reference);

            }
        }
    }
}
