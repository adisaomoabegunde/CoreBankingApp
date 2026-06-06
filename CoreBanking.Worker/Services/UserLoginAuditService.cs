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
    public class UserLoginAuditService : IUserLoginAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<UserLoginAuditService> _logger;

        public UserLoginAuditService(IAuditRepository auditRepository, ILogger<UserLoginAuditService> logger)
        {
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task HandleAsync(UserLoggedInEvent loggedInEvent)
        {
            try
            {
                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "User Logged In",
                    Description = $"User {loggedInEvent.Email} logged in from IP {loggedInEvent.IpAddress}",
                    IpAddress = loggedInEvent.IpAddress,
                    Timestamp = DateTime.UtcNow,
                    UserId = loggedInEvent.UserId
                };
                await _auditRepository.AddAsync(audit);

                _logger.LogInformation("Login audit log created for user: {UserId}", loggedInEvent.UserId);
            }
            catch(Exception ex)
            {
                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    Action = "user Log in failed",
                    Description = $"User {loggedInEvent.Email} logged in from IP {loggedInEvent.IpAddress} failed",
                    IpAddress = loggedInEvent.IpAddress,
                    Timestamp = DateTime.UtcNow,
                    UserId = loggedInEvent.UserId
                });
                _logger.LogInformation("Login audit log failed for user: {UserId}", loggedInEvent.UserId);
            }
        }
    }
}
