using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class UserNotificationServiceb : IUserNotificationService
    {
        private readonly ILogger<UserNotificationServiceb> _logger;

        public UserNotificationServiceb(ILogger<UserNotificationServiceb> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UserRegisteredEvent userRegisteredEvent)
        {
            _logger.LogInformation("Welcome email sent to {Email} for UserId: {UserId}", userRegisteredEvent.Email, userRegisteredEvent.UserId);
            return Task.CompletedTask;
        }
    }
}
