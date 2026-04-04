using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Transactions
{
    public class TransferCommand : IRequest<ApiResponse<string>>
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string IdempotencyKey { get; set; }
    }
}
