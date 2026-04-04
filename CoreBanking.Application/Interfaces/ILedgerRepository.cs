using CoreBanking.Domain.Entities;

public interface ILedgerRepository
{
    Task AddAsync(LedgerEntry entry);
    Task AddRangeAsync(List<LedgerEntry> entries);

    Task<(List<LedgerEntry>, int)> GetAccountStatementAsync(
        Guid accountId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize
    );

    Task<List<LedgerEntry>> GetByDateRangeAsync(DateTime start, DateTime end);

}