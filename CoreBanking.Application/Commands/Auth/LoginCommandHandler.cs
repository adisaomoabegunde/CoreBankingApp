using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
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

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService, IAuditRepository auditRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _auditRepository = auditRepository;
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
                        .FailureResponse("Invalid email or password.", "01");
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
                        .FailureResponse("Account is inactive. Please contact support.", "02");
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
                        .FailureResponse("Invalid email or password.", "01");

                }

                var token = _jwtService.GenerateToken(user);
                await _auditRepository.AddAsync(new AuditLog
                {
                    UserId = user.Id,
                    Action = "Login Success",
                    IpAddress = ipAddress,
                    Description = "User logged in successfully"
                });


                var response = new AuthResponse
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role
                };

                return ApiResponse<AuthResponse>
                    .SuccessResponse(response, "Login successful.");

            }
            catch (Exception ex)
            { 
                return ApiResponse<AuthResponse>
                    .FailureResponse(ex.Message, "99");



            }



           
            
           
        }
    }
}
