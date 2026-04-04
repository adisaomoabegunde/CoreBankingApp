using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Accounts
{
    public class GetAccountDetailsQueryHandler : IRequestHandler<GetAccountDetailsQuery, ApiResponse<GetAccountDetailsReponse>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAccountDetailsQueryHandler> _logger;

        public GetAccountDetailsQueryHandler(IAccountRepository accountRepository, ICustomerRepository customerRepository, ICurrentUserService currentUserService, ILogger<GetAccountDetailsQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task<ApiResponse<GetAccountDetailsReponse>> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                _logger.LogInformation("Fetching account details for user {userId} and account {accountNumber}", userId, request.AccountNumber);

                var account = await _accountRepository
                    .GetByAccountNumberAsync(request.AccountNumber);
                if (account == null)
                {
                    _logger.LogWarning("Account not found: {AccountNumber}", request.AccountNumber);
                    return ApiResponse<GetAccountDetailsReponse>.NotFound("Account not found");
                }

                var customer = await _customerRepository.GetByUserIdAsync(userId);
                if (customer != null && account.CustomerId != customer.Id)
                {
                    _logger.LogWarning("Unauthorized access attempt by user {userId} to account {accountNumber}", userId, request.AccountNumber);
                    return ApiResponse<GetAccountDetailsReponse>.Unauthorized("You do not have access to this account");
                }

                var response = new GetAccountDetailsReponse
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    Balance = account.Balance,
                    Currency = account.Currency,
                    Status = account.Status,
                    DailyTransferLimit = account.DailyTransferlimit,
                    MinimumBalance = account.MinimumBalance,
                    DateOpened = account.DateOpened,
                    LasTransactionDate = account.LastTransactionDate
                };
                _logger.LogInformation("Successfully fetched account details for account {accountNumber}", request.AccountNumber);
                return ApiResponse<GetAccountDetailsReponse>
                    .SuccessResponse(response, "Account details retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account details for account {accountNumber}", request.AccountNumber);
                return ApiResponse<GetAccountDetailsReponse>
                    .InternalServerError("An error occurred while retrieving account details");
            }
        }
    }
}
