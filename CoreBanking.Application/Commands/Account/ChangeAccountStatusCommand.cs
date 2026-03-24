using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Account
{
    public class ChangeAccountStatusCommand : IRequest<ApiResponse<string>>
    {
        [JsonIgnore]
        public string? AccountNumber { get; set; }
        public AccountStatus Status { get; set; }
    }
}
