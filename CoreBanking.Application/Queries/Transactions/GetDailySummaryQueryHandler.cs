using CoreBanking.Application.DTOs;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Transactions
{
    public class GetDailySummaryQueryHandler
     : IRequestHandler<GetDailySummaryQuery, ApiResponse<DailyTransactionSummaryDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILedgerRepository _ledgerRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<GetDailySummaryQueryHandler> _logger;

        public GetDailySummaryQueryHandler(
            ITransactionRepository transactionRepository,
            ILedgerRepository ledgerRepository,
            ICurrentUserService currentUser,
            ILogger<GetDailySummaryQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _ledgerRepository = ledgerRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ApiResponse<DailyTransactionSummaryDto>> Handle(
            GetDailySummaryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching daily transaction summary");

                // 🔐 ADMIN ONLY
                if (_currentUser.Role != "Admin")
                {
                    _logger.LogWarning("Unauthorized access attempt by user {UserId}", _currentUser.UserId);
                    return ApiResponse<DailyTransactionSummaryDto>
                        .Unauthorized("Only admins can access this report");
                }

                // 📅 Date handling (UTC safe)
                var date = request.Date ?? DateTime.UtcNow;

                var start = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
                var end = start.AddDays(1);

                _logger.LogInformation("Fetching data between {Start} and {End}", start, end);

                // 🔹 Get data
                var transactions = await _transactionRepository
                    .GetByDateRangeAsync(start, end);

                var ledgerEntries = await _ledgerRepository
                    .GetByDateRangeAsync(start, end);

                // 🔥 DEBUG LOGS (important for your 0 issue)
                _logger.LogInformation("Transactions count: {Count}", transactions.Count);
                _logger.LogInformation("Ledger entries count: {Count}", ledgerEntries.Count);

                // 🔹 Aggregations
                var totalTransactions = transactions.Count;

                var totalDeposits = transactions
                    .Where(t => t.TransactionType == TransactionType.Deposit)
                    .Sum(t => t.Amount);

                var totalWithdrawals = transactions
                    .Where(t => t.TransactionType == TransactionType.Withdrawal)
                    .Sum(t => t.Amount);

                var totalTransfers = transactions
                    .Where(t => t.TransactionType == TransactionType.Transfer)
                    .Sum(t => t.Amount);

                var totalCredits = ledgerEntries
                    .Where(l => l.EntryType == EntryType.Credit)
                    .Sum(l => l.Amount);

                var totalDebits = ledgerEntries
                    .Where(l => l.EntryType == EntryType.Debit)
                    .Sum(l => l.Amount);

                var response = new DailyTransactionSummaryDto
                {
                    Date = start,
                    TotalTransactions = totalTransactions,
                    TotalDeposits = totalDeposits,
                    TotalWithdrawals = totalWithdrawals,
                    TotalTransfers = totalTransfers,
                    TotalCredits = totalCredits,
                    TotalDebits = totalDebits
                };

                _logger.LogInformation("Daily summary generated successfully");

                return ApiResponse<DailyTransactionSummaryDto>
                    .SuccessResponse(response, "Daily summary retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching daily summary");

                return ApiResponse<DailyTransactionSummaryDto>
                    .InternalServerError("An error occurred while retrieving daily summary");
            }
        }
    }
}
