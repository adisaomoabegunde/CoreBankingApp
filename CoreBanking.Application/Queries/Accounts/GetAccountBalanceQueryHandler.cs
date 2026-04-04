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
    public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, ApiResponse<GetAccountBalanceResponse>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAccountBalanceQuery> _logger;

        public GetAccountBalanceQueryHandler(IAccountRepository accountRepository, ICustomerRepository customerRepository, ICurrentUserService currentUserService, ILogger<GetAccountBalanceQuery> logger)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ApiResponse<GetAccountBalanceResponse>> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                _logger.LogInformation("Fetching account balance. AccountNumber: {AccountNumber}, UserId: {UserId}", request.AccountNumber, userId);



                var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber);
                if (account == null)
                {
                    _logger.LogWarning("Account not found. AccountNumber: {AccountNumber}", request.AccountNumber);
                    return ApiResponse<GetAccountBalanceResponse>.NotFound("Account not found");
                }

                var customer = await _customerRepository.GetByUserIdAsync(userId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer profile not found for UserId: {UserId}", userId);

                    return ApiResponse<GetAccountBalanceResponse>
                        .Unauthorized("Customer profile not found");
                }

                if (account.CustomerId != customer.Id)
                {
                    _logger.LogWarning(
                        "Unauthorized balance access attempt. UserId: {UserId}, Account: {AccountNumber}",
                        userId, request.AccountNumber);

                    return ApiResponse<GetAccountBalanceResponse>
                        .Unauthorized("You can only access your own account");
                }


                var response = new GetAccountBalanceResponse
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance,
                    Currency = account.Currency.ToString(),
                };
                _logger.LogInformation("Account balance fetched successfully. AccountNumber: {AccountNumber}, Balance: {Balance} {Currency}", account.AccountNumber, account.Balance, account.Currency);
                return ApiResponse<GetAccountBalanceResponse>
                    .SuccessResponse(response, "balance retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account balance. AccountNumber: {AccountNumber}", request.AccountNumber);
                return ApiResponse<GetAccountBalanceResponse>
                    .InternalServerError("An error occurred while fetching the account balance");
            }
        }
    }
}
