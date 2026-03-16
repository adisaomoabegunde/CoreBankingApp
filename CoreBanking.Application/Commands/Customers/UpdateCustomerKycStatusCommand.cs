using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Customers
{
    public class UpdateCustomerKycStatusCommand : IRequest<ApiResponse<string>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public KycStatus KycStatus { get; set; }
    }
}
