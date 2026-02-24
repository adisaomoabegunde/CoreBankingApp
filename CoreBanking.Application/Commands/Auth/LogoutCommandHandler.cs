using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public LogoutCommandHandler(ITokenBlacklistRepository tokenBlacklistRepository, IAuditRepository auditRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _auditRepository = auditRepository;
            _httpContextAccessor = httpContextAccessor;
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
