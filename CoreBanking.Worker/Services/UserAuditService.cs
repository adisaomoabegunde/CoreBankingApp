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
    public class UserAuditService : IUserAuditService
    {
        private readonly IAuditRepository _auditRepository;  
        private readonly ILogger<UserAuditService> _logger;

        public UserAuditService(IAuditRepository auditRepository, ILogger<UserAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }


        public async Task HandleAsync(UserRegisteredEvent userRegisteredEvent)
        {
            try
            {
                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "User Registered",
                    Description = $"User registered with email {userRegisteredEvent.Email}",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = userRegisteredEvent.UserId
                };
                await _auditRepository.AddAsync(audit);
                _logger.LogInformation("User registration audit log created: {UserId}", userRegisteredEvent.UserId);
            }
            catch (Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "User Registered failed",
                    Description = $"User registered with Email {userRegisteredEvent.Email} failed",
                    IpAddress = "System",
                    Timestamp = DateTime.UtcNow,
                    UserId = userRegisteredEvent.UserId
                });

                _logger.LogError(ex, "User registration audit log failed: {UserId}", userRegisteredEvent.UserId);
            }
        }

    }
}
