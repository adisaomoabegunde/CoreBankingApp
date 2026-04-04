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
    public class GetAuditTrailQuery : IRequest<ApiResponse<AuditLogListDto>>
    {
        public Guid? UserId { get; set; }
        public string? Action {  get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
