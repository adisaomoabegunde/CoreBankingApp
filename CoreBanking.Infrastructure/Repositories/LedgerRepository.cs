using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBanking.Infrastructure.Persistence.Repositories
{
    public class LedgerRepository : ILedgerRepository
    {
        private readonly AppDbContext _context;

        public LedgerRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add single ledger entry
        public async Task AddAsync(LedgerEntry entry)
        {
            await _context.LedgerEntries.AddAsync(entry);
        }

        // ✅ Add multiple ledger entries
        public async Task AddRangeAsync(List<LedgerEntry> entries)
        {
            await _context.LedgerEntries.AddRangeAsync(entries);
        }

        // ✅ ACCOUNT STATEMENT (CORE METHOD 🔥)
        public async Task<(List<LedgerEntry>, int)> GetAccountStatementAsync(
            Guid accountId,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize)
        {
            // Base query
            var query = _context.LedgerEntries
                .Include(l => l.Transaction)
                .Where(l => l.AccountId == accountId);

            // ✅ Date filtering (IMPORTANT: UTC FIX)
            if (fromDate.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc);
                query = query.Where(l => l.CreatedAt >= fromUtc);
            }

            if (toDate.HasValue)
            {
                var toUtc = DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc);
                query = query.Where(l => l.CreatedAt <= toUtc);
            }

            // ✅ Total count (before pagination)
            var totalRecords = await query.CountAsync();

            // ✅ Pagination + ordering
            var data = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<List<LedgerEntry>> GetByDateRangeAsync(
            DateTime start,
            DateTime end)
                {
                    return await _context.LedgerEntries
                        .Where(l => l.CreatedAt >= start && l.CreatedAt < end)
                        .ToListAsync();
                }
    }
}