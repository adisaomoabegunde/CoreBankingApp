using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<string>>
    {
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventProducer _eventProducer;
        private readonly ILogger<LogoutCommandHandler> _logger;
        public LogoutCommandHandler(ITokenBlacklistRepository tokenBlacklistRepository, IAuditRepository auditRepository, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService, IEventProducer eventProducer, ILogger<LogoutCommandHandler> logger)
        {
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _auditRepository = auditRepository;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _eventProducer = eventProducer;
            _logger = logger;
        }
        public async Task<ApiResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {


            try
            {

                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
               .ToString()
               .Replace("Bearer ", "");


                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var expiry = jwtToken.ValidTo;

                var userId = _currentUserService.UserId;

                await _tokenBlacklistRepository.AddAsync(new RevokedToken
                {
                    Token = token,
                    ExpiryDate = expiry
                });

                await _auditRepository.AddAsync(new AuditLog
                {
                    Action = "Logout",
                    Description = "User logged out",
                    IpAddress = "N/A"
                });

                try
                {
                    var logoutEvent = new UserLoggedOutEvent
                    {
                        UserId = userId,
                        IpAddress = _currentUserService.IpAdress,
                        LoggedOutAt = DateTime.UtcNow,
                        Reference = $"LOGOUT-{DateTime.UtcNow.Ticks}"

                    };
                    await _eventProducer.PublishAsync("user.loggedout", logoutEvent);

                    _logger.LogInformation("User logout event published: {Reference}", logoutEvent.Reference);

                }
                catch(Exception KafkaEx)
                {
                    _logger.LogError(KafkaEx, "Failed to publish user logout event");
                }

                return ApiResponse<string>
                    .SuccessResponse("Logout successful", "00");


            }
            catch (Exception ex)
            {
                return ApiResponse<string>
                    .InternalServerError("Logout failed: " + ex.Message);
            }





            

        }
    }
}
