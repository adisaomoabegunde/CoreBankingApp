using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Accounts
{
    public class GetCustomerAccountsQueryHandler : IRequestHandler<GetCustomerAccountsQuery, ApiResponse<List<GetCustomerAccountsResponse>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerAccountsQueryHandler> _logger;

        public GetCustomerAccountsQueryHandler(IAccountRepository accountRepository, ICustomerRepository customerRepository, ICurrentUserService currentUserService, ILogger<GetCustomerAccountsQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetCustomerAccountsResponse>>> Handle(GetCustomerAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                _logger.LogInformation("Fetching accounts for CustomerId: {CustomerId}", request.CustomerId);

                var currentCustomer = await _customerRepository
                    .GetByUserIdAsync(currentUserId);

                if (currentCustomer != null && currentCustomer.Id != request.CustomerId)
                {
                    _logger.LogWarning("Unauthorized access attempt. UserId: {UserId} tried to access CustomerId: {CustomerId}", currentUserId, request.CustomerId);
                    return ApiResponse<List<GetCustomerAccountsResponse>>.Unauthorized("You do not have access to these accounts");
                }
                var accounts = await _accountRepository
                    .GetByCustomerIdAsync(request.CustomerId);

                if(accounts == null || !accounts.Any())
                {
                    _logger.LogInformation("No accounts found for CustomerId: {CustomerId}", request.CustomerId);
                    return ApiResponse<List<GetCustomerAccountsResponse>>
                        .NotFound("No accounts found for the specified customer");
                }   



                var response = accounts.Select(a => new GetCustomerAccountsResponse
                {

                    AccountNumber = a.AccountNumber,
                    AccountType = a.AccountType,
                    Balance = a.Balance,
                    Currency = a.Currency,
                    Status = a.Status,
                    DateOpened = a.DateOpened
                }).ToList();
                _logger.LogInformation("Successfully fetched {AccountCount} accounts for CustomerId: {CustomerId}", response.Count, request.CustomerId);
                return ApiResponse<List<GetCustomerAccountsResponse>>
                    .SuccessResponse(response, "Customer accounts retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts for CustomerId: {CustomerId}", request.CustomerId);
                return ApiResponse<List<GetCustomerAccountsResponse>>
                    .InternalServerError("An error occurred while retrieving customer accounts");
            }
        }
    }
}
