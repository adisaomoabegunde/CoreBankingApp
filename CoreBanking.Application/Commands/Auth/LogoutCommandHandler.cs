using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, string>
    {
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly IAuditRepository _auditRepository;

        public LogoutCommandHandler(ITokenBlacklistRepository tokenBlacklistRepository, IAuditRepository auditRepository)
        {
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _auditRepository = auditRepository;
        }
        public async Task<string> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(request.Token);

            var expiry = jwtToken.ValidTo;

            await _tokenBlacklistRepository.AddAsync(new RevokedToken
            {
                Token = request.Token,
                ExpiryDate = expiry
            });

            await _auditRepository.AddAsync(new AuditLog
            {
                Action = "Logout",
                Description = "User logged out",
                IpAddress = "N/A"
            });
            return "Logout successful.";

        }
    }
}
