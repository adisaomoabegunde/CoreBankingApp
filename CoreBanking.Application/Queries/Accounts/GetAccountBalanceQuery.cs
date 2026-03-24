using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Accounts
{
    public class GetAccountBalanceQuery : IRequest<ApiResponse<GetAccountBalanceResponse>>
    {
        public string AccountNumber { get; set; }

        public GetAccountBalanceQuery(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }
}
