using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Worker.Services
{
    public class UserLoginCacheService : IUserLoginCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<UserLoginCacheService> _logger;

        public UserLoginCacheService(IConnectionMultiplexer redis, ILogger<UserLoginCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }
        public async Task HandleAsync(UserLoggedInEvent loggedInEvent)
        {
            var key = $"user:{loggedInEvent.UserId}:lastLogin";

            await _db.StringSetAsync(key, loggedInEvent.LoginAt.ToString("O"));

            _logger.LogInformation("User last login updated is Redis: {USerId}", loggedInEvent.UserId);
        }
    }
}
