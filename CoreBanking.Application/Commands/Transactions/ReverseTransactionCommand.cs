using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Transactions
{
    public class ReverseTransactionCommand : IRequest<ApiResponse<string>>
    {
        public string Reference { get; set; }
        public ReverseTransactionCommand(string reference)
        {
            Reference = reference;
        }
    }
}
