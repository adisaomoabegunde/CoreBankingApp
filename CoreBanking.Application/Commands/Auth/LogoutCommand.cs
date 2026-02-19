using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class LogoutCommand : IRequest<ApiResponse<string>>
    {
        //public string Token { get; set; } = default!;
    }
}
