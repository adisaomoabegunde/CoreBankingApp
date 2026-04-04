using CoreBanking.Application.DTOs;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;
using CoreBanking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Transactions;

namespace CoreBanking.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task AddRangeAsync(IEnumerable<LedgerEntry> ledgerEntries)
        {
            await _context.LedgerEntries.AddRangeAsync(ledgerEntries);
        }
        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
        }
        public async Task<int> GetTodayTransferCountAsync(Guid accountId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Transactions
                .CountAsync(t =>
                    t.SourceAccountId == accountId &&
                    t.TransactionType == Domain.Enums.TransactionType.Transfer &&
                    t.TransactionDate.Date == today);
        }

        public async Task<decimal> GetTodayTransferTotalAsync(Guid accountId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Transactions
                .Where(t => 
                    t.SourceAccountId == accountId &&
                    t.TransactionType == Domain.Enums.TransactionType.Transfer &&
                    t.TransactionDate.Date == today)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<decimal> GetTodayWithdrawalTotalAsync(Guid accountId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Transactions
                .Where(t =>
                    t.SourceAccountId == accountId &&
                    t.TransactionType == Domain.Enums.TransactionType.Withdrawal &&
                    t.TransactionDate.Date == today)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<(List<Transaction>, int)> GetByAccountAsync(Guid accountId, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            var query = _context.Transactions
                .Where(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId);
            if (fromDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= toDate.Value);
            }
            var totalRecords = await query.CountAsync();

            var transactions = await query
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (transactions, totalRecords);
        }
        public async Task<Transaction?> GetByReferenceAsync(string reference)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionReference == reference);
        }

        public async Task<List<Transaction>> GetByDateRangeAsync(
            DateTime start,
            DateTime end)
                {
                    return await _context.Transactions
                        .Where(t => t.TransactionDate >= start && t.TransactionDate < end)
                        .ToListAsync();
                }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
