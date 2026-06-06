using CoreBanking.Application.Events;
using CoreBanking.Worker.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace CoreBanking.Worker.Services
{
    public class UserLogoutCacheService : IUserLogoutCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<UserLogoutCacheService> _logger;

        public UserLogoutCacheService(IConnectionMultiplexer redis, ILogger<UserLogoutCacheService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }
        
        public async Task HandleAsync(UserLoggedOutEvent loggedOutEvent)
        {
            var sessionKey = $"user:{loggedOutEvent.UserId}:session";
            var refreshTokenKey = $"user:{loggedOutEvent.UserId}:refreshToken";

            await _db.KeyDeleteAsync(sessionKey);
            await _db.KeyDeleteAsync(refreshTokenKey);

            _logger.LogInformation("User session cleared from Redis: {UserId}", loggedOutEvent.UserId);
        }

    }
}
