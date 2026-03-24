using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Accounts
{
    public class GetAccountDetailsQuery : IRequest<ApiResponse<GetAccountDetailsReponse>>
    {
        public string AccountNumber { get; set; }

        public GetAccountDetailsQuery(string accountNumber)
        {
            AccountNumber = accountNumber;
        }
    }
}
