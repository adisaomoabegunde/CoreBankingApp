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
    public class ReversalAuditService : IReversalAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<ReversalAuditService> _logger;

        public ReversalAuditService(IAuditRepository auditRepository, ILogger<ReversalAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(TransactionReversedEvent reversedEvent)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Transaction Reversal",
                    Description = $"Transaction with reference {reversedEvent.Reference} was reversed.",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System",
                    UserId = Guid.Empty
                };
                await _auditRepository.AddAsync(auditLog);
                _logger.LogInformation("Audit log created for reversal of {Reference}", reversedEvent.Reference);
            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Reversal Audit Failed",
                    Description = $"Failed to create audit log for reversal of transaction with reference {reversedEvent.Reference}. Error: {ex.Message}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "System",
                    UserId = Guid.Empty
                });
                _logger.LogError(ex, "Failed to create audit log for reversal of {Reference}", reversedEvent.Reference);

            }
        }
    }
}
