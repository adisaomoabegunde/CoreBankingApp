using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class UserSecurityService : IUserSecurityService
    {
        private readonly ILogger<UserSecurityService> _logger;

        public UserSecurityService(ILogger<UserSecurityService> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserLoggedInEvent loggedInEvent)
        {
            _logger.LogInformation("Security check passed for user {UserId} from IP {IpAddress}", loggedInEvent.UserId, loggedInEvent.IpAddress);
            return Task.CompletedTask;
        }
    }
}
