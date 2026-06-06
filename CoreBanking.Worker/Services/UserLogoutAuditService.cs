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
    public class UserLogoutAuditService : IUserLogoutAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<UserLogoutAuditService> _logger;

        public UserLogoutAuditService(IAuditRepository auditRepository, ILogger<UserLogoutAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(UserLoggedOutEvent loggedOutEvent)
        {
            try
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "User Logged Out",
                    Description = $"USer logged out from IP {loggedOutEvent.IpAddress}",
                    IpAddress = loggedOutEvent.IpAddress,
                    Timestamp = DateTime.UtcNow,
                    UserId = loggedOutEvent.UserId
                });
                _logger.LogInformation("Logout audit log created for user: {UserId}", loggedOutEvent.UserId);
            }
            catch(Exception ex)
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "User logged out failed",
                    Description = $"User logged out from IP {loggedOutEvent.IpAddress} failed",
                    IpAddress = loggedOutEvent.IpAddress,
                    Timestamp = DateTime.UtcNow,
                    UserId = loggedOutEvent.UserId
                });
                _logger.LogError(ex, "Logout audit log failed for user {UserId}", loggedOutEvent.UserId);
            }
        }
    }
}
