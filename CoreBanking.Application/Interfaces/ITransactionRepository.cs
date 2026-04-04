using CoreBanking.Application.DTOs;
using CoreBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = CoreBanking.Domain.Entities.Transaction;

namespace CoreBanking.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Domain.Entities.Transaction transaction);
        Task AddRangeAsync(IEnumerable<LedgerEntry> ledgerEntries);
        Task<Domain.Entities.Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);

        Task<int> GetTodayTransferCountAsync(Guid accountId);
        Task<decimal> GetTodayTransferTotalAsync(Guid accountId);
        Task<decimal> GetTodayWithdrawalTotalAsync(Guid accountId);
        Task<(List<Transaction>, int)> GetByAccountAsync(
            Guid accountId,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize
            );
        Task<Transaction?> GetByReferenceAsync(string reference);
        Task<List<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end);

        Task SaveChangesAsync();
    }
}
