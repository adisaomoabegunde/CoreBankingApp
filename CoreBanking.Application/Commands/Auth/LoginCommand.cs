using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class LoginCommand : IRequest<ApiResponse<AuthResponse>>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
    }
}
