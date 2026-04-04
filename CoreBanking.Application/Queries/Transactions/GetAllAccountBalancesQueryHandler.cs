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
    public class GetAllAccountBalancesQueryHandler
    : IRequestHandler<GetAllAccountBalancesQuery, ApiResponse<AccountBalanceListDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<GetAllAccountBalancesQueryHandler> _logger;

        public GetAllAccountBalancesQueryHandler(
            IAccountRepository accountRepository,
            ICurrentUserService currentUser,
            ILogger<GetAllAccountBalancesQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ApiResponse<AccountBalanceListDto>> Handle(
            GetAllAccountBalancesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all account balances");

                // 🔐 ADMIN ONLY
                if (_currentUser.Role != "Admin")
                {
                    _logger.LogWarning("Unauthorized access by user {UserId}", _currentUser.UserId);

                    return ApiResponse<AccountBalanceListDto>
                        .Unauthorized("Only admins can access this report");
                }

                // 📊 Get data
                var (accounts, totalRecords) = await _accountRepository
                    .GetAllWithPaginationAsync(request.PageNumber, request.PageSize);

                // 🔄 Map to DTO
                var result = accounts.Select(a => new AccountBalanceDto
                {
                    AccountNumber = a.AccountNumber,
                    CustomerName = a.Customer.FirstName + " " + a.Customer.LastName,
                    Balance = a.Balance,
                    Status = a.Status.ToString()
                }).ToList();

                var response = new AccountBalanceListDto
                {
                    Accounts = result,
                    TotalRecords = totalRecords
                };

                _logger.LogInformation("Successfully retrieved {Count} accounts", result.Count);

                return ApiResponse<AccountBalanceListDto>
                    .SuccessResponse(response, "Account balances retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching account balances");

                return ApiResponse<AccountBalanceListDto>
                    .InternalServerError("An error occurred while retrieving account balances");
            }
        }
    }
}
