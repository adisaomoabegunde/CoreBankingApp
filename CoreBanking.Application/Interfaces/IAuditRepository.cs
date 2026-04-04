using CoreBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Interfaces
{
    public interface IAuditRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<(List<AuditLog>, int)> GetAuditLogsAsync(
            Guid? userId,
            string? action,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize);
            }
}
