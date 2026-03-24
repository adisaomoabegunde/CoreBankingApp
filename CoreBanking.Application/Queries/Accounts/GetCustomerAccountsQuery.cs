using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Accounts
{
    public class GetCustomerAccountsQuery : IRequest<ApiResponse<List<GetCustomerAccountsResponse>>>
    {
        public Guid CustomerId { get; set; }
        public GetCustomerAccountsQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
