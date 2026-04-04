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
    public class GetAccountStatementQueryHandler : IRequestHandler<GetAccountStatementQuery, ApiResponse<AccountStatementDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILedgerRepository _ledgerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<GetAccountStatementQueryHandler> _logger;

        public GetAccountStatementQueryHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, ILedgerRepository ledgerRepository, ICustomerRepository customerRepository, ICurrentUserService currentUser, ILogger<GetAccountStatementQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _ledgerRepository = ledgerRepository;
            _customerRepository = customerRepository;
            _currentUser = currentUser;
            _logger = logger;
        }
        public async Task<ApiResponse<AccountStatementDto>> Handle(GetAccountStatementQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Accountstatement for account {Account}", request.AccountNumber);

                var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber);
                if(account == null)
                {
                    return ApiResponse<AccountStatementDto>.NotFound("Account not found");

                }
                var userId = _currentUser.UserId;
                var customer = await _customerRepository.GetByUserIdAsync(userId);
                if (customer != null && account.CustomerId != customer.Id)

                {
                    _logger.LogWarning("Unauthorized access attempt by user {userId} to Account Statement of account {accountNumber}", userId, request.AccountNumber);
                    return ApiResponse<AccountStatementDto>
                        .Unauthorized("You do not have access to this account's transactions");
                }

                var (entries, totalRecords) = await _ledgerRepository
                    .GetAccountStatementAsync(
                        account.Id,
                        request.FromDate,
                        request.ToDate,
                        request.PageNumber,
                        request.PageSize
                        );
                var transactions = entries.Select(e => new TransactionStatementItemDto
                {
                    Reference = e.Transaction.TransactionReference,
                    Type = e.EntryType.ToString(),
                    Amount = e.Amount,
                    BalanceAfter = e.BalanceAfter,
                    Date = e.CreatedAt,
                    Description = e.Description
                }).ToList();

                var response = new AccountStatementDto
                {
                    AccountNumber = account.AccountNumber,
                    CurrentBalance = account.Balance,
                    Transactions = transactions,
                    TotalRecords = totalRecords
                };

                _logger.LogInformation("Statement retrieved successfully for account {accountNumber}", request.AccountNumber);
                return ApiResponse<AccountStatementDto>
                    .SuccessResponse(response, "Statement retrieved successfully");

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error fetching Account statement for account {Account}", request.AccountNumber);
                return ApiResponse<AccountStatementDto>
                    .InternalServerError("Failed to retrieve Account Statement: " + ex.Message);
            }
        }
    }
}
