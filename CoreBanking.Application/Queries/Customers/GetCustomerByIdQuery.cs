using CoreBanking.Application.DTOs;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Customers
{
    public class GetCustomerByIdQuery : IRequest<ApiResponse<CustomerDto>>
    {
        public Guid Id { get; set; }
        public GetCustomerByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
