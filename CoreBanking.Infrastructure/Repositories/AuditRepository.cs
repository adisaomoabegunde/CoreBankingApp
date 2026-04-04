using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Infrastructure.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AppDbContext _context;

        public AuditRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
        public async Task<(List<AuditLog>, int)> GetAuditLogsAsync(Guid? userId, string? action, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
                var query = _context.AuditLogs.AsQueryable();

                // 🔍 Filters
                if (userId.HasValue)
                    query = query.Where(x => x.UserId == userId);

                if (!string.IsNullOrWhiteSpace(action))
                    query = query.Where(x => x.Action.ToLower().Contains(action.ToLower()));

                if (fromDate.HasValue)
                {
                    var fromUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
                    query = query.Where(x => x.Timestamp >= fromUtc);
                }

                if (toDate.HasValue)
                {
                    var toUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);
                    query = query.Where(x => x.Timestamp <= toUtc);
                }

                var totalRecords = await query.CountAsync();

                var data = await query
                    .OrderByDescending(x => x.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (data, totalRecords);
        }

    }
}
