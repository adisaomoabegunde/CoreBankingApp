using CoreBanking.Application.DTOs;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Transactions
{
    public class GetTransactionByReferenceQuery : IRequest<ApiResponse<TransactionDto>>
    {
        public string Reference { get; set; }

        public GetTransactionByReferenceQuery(string reference)
        {
            Reference = reference;
        }
    }
}
