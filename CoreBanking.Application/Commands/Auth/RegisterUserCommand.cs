using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class RegisterUserCommand : IRequest<ApiResponse<RegisterUserResponse>>
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public UserRole Role { get; set; } 
    }
}
