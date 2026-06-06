using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        private readonly IAuditRepository _auditRepository;
        private readonly IEventProducer _eventProducer;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService, IAuditRepository auditRepository, IEventProducer eventProducer, ILogger<LoginCommandHandler> logger )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _auditRepository = auditRepository;
            _eventProducer = eventProducer;
            _logger = logger;
        }
        public async Task<ApiResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var ipAddress = request.IpAddress;
                var user = await _userRepository.GetByEmailAsync(request.Email);


                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    await _auditRepository.AddAsync(new AuditLog
                    {
                        Action = "Login Failed",
                        IpAddress = ipAddress,
                        Description = $"Failed login attempt for email: {request.Email}"
                    });
                    return ApiResponse<AuthResponse>
                        .Unauthorized("Invalid email or password.");
                }


                if (!user.IsActive)
                {

                    await _auditRepository.AddAsync(new AuditLog
                    {
                        UserId = user.Id,
                        Action = "Login Failed - Inactive Account",
                        IpAddress = ipAddress,
                        Description = $"Login attempt for inactive account: {request.Email}"
                    });
                    return ApiResponse<AuthResponse>
                        .BadRequest("Account is inactive. Please contact support.");
                }


                var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                if (!passwordValid)
                {
                    await _auditRepository.AddAsync(new AuditLog
                    {
                        UserId = user.Id,
                        Action = "Login Failed - Invalid Password",
                        IpAddress = ipAddress,
                        Description = "Incorrect password"
                    });
                    return ApiResponse<AuthResponse>
                        .Unauthorized("Invalid email or password.");

                }

                var token = _jwtService.GenerateToken(user);
               

                try
                {
                    var loginEvent = new UserLoggedInEvent
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        IpAddress = ipAddress,
                        LoginAt = DateTime.UtcNow,
                        Reference = $"LOGIN-{DateTime.UtcNow.Ticks}"
                    };
                    await _eventProducer.PublishAsync("user.loggedin", loginEvent);
                    _logger.LogInformation("User login event published: {Reference}", loginEvent.Reference);
                }
                catch(Exception KafkaEx)
                {
                    _logger.LogError(KafkaEx, "Failed to publish user login event");
                }

                var response = new AuthResponse
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role.ToString()
                };

                return ApiResponse<AuthResponse>
                    .SuccessResponse(response, "Login successful.");

            }
            catch (Exception ex)
            { 
                return ApiResponse<AuthResponse>
                    .InternalServerError(ex.Message);



            }



           
            
           
        }
    }
}
