using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Customers
{
    public class UpdateCustomerKycStatusCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; }
        public string KycStatus { get; set; }
    }
}
