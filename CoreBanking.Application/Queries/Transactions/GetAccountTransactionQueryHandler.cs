using CoreBanking.Application.DTOs;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Transactions
{
    public class GetAccountTransactionQueryHandler : IRequestHandler<GetAccountTransactionQuery, ApiResponse<PagedResultt<TransactionDto>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<GetAccountTransactionQueryHandler> _logger;

        public GetAccountTransactionQueryHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICustomerRepository customerRepository, ICurrentUserService currentUser, ILogger<GetAccountTransactionQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResultt<TransactionDto>>> Handle(GetAccountTransactionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching transactions for account {Account}", request.AccountNumber);

                var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber);
                if (account == null)
                {
                    return ApiResponse<PagedResultt<TransactionDto>>
                        .NotFound("Account not found");
                }

                var userId = _currentUser.UserId;
                var customer = await _customerRepository.GetByUserIdAsync(userId);

                if (customer != null && account.CustomerId != customer.Id)
                {
                    _logger.LogWarning("Unauthorized access attempt by user {userId} to transactions of account {accountNumber}", userId, request.AccountNumber);
                    return ApiResponse<PagedResultt<TransactionDto>>
                        .Unauthorized("You do not have access to this account's transactions");
                }
                var fromDate = request.FromDate.HasValue
                    ? DateTime.SpecifyKind(request.FromDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;

                var toDate = request.ToDate.HasValue
                    ? DateTime.SpecifyKind(request.ToDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null;

                var (transactions, totalRecords) =
                    await _transactionRepository.GetByAccountAsync(
                        account.Id,
                        fromDate,
                        toDate,
                        request.PageNumber,
                        request.PageSize);

                if (totalRecords == 0)
                {
                    return ApiResponse<PagedResultt<TransactionDto>>
                        .SuccessResponse(new PagedResultt<TransactionDto>
                        {
                            PageNumber = request.PageNumber,
                            PageSize = request.PageSize,
                            TotalRecords = 0,
                            Data = new List<TransactionDto>()
                        }, "No transactions found for this account");
                }

                var result = transactions.Select(t => new TransactionDto
                {
                    TransactionReference = t.TransactionReference,
                    TransactionType = t.TransactionType.ToString(),
                    Amount = t.Amount,
                    Status = t.Status.ToString(),
                    SourceAccountNumber = t.SourceAccount?.AccountNumber,
                    DestinationAccountNumber = t.DestinationAccount?.AccountNumber,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description
                });

                return ApiResponse<PagedResultt<TransactionDto>>
                    .SuccessResponse(new PagedResultt<TransactionDto>
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalRecords = totalRecords,
                        Data = result.ToList()
                    }, "Transactions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transactions for account {Account}", request.AccountNumber);
                return ApiResponse<PagedResultt<TransactionDto>>
                    .InternalServerError("Failed to retrieve Transactions: " + ex.Message);

            }
        }
    }
}
