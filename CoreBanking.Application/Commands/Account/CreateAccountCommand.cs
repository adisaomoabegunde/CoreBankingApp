using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Account
{
    public class CreateAccountCommand : IRequest<ApiResponse<string>>
    {
        public AccountType AccountType { get; set; }
        public decimal InitialDeposit {  get; set; }
        public Currency Currency { get; set; }

    }
}
