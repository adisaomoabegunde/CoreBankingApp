using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    internal class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        private readonly IAuditRepository _auditRepository;

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService, IAuditRepository auditRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _auditRepository = auditRepository;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var ipAddress = request.IpAddress;

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                await _auditRepository.AddAsync(new Domain.Entities.AuditLog
                {
                    Action = "Login Failed",
                    IpAddress = ipAddress,
                    Description = $"Failed login attempt for email: {request.Email}"
                });
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            if (!user.IsActive)
            {

                await _auditRepository.AddAsync(new AuditLog {
                    UserId = user.Id,
                    Action = "Login Failed - Inactive Account",
                    IpAddress = ipAddress,
                    Description = $"Login attempt for inactive account: {request.Email}"
                });
                throw new UnauthorizedAccessException("Account is inactive.");
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

                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var token = _jwtService.GenerateToken(user);
            await _auditRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "Login Success",
                IpAddress = ipAddress,
                Description = "User logged in successfully"
            });
            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}
