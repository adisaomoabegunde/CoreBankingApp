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
    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, ApiResponse<List<GetAllAccountsReponse>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<GetAllAccountsQueryHandler> _logger;

        public GetAllAccountsQueryHandler(IAccountRepository accountRepository, ILogger<GetAllAccountsQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }
        public async Task<ApiResponse<List<GetAllAccountsReponse>>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all accounts");

                var accounts = await _accountRepository.GetAllAsync();

                var response = accounts.Select(a => new GetAllAccountsReponse
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountType = a.AccountType,
                    Balance = a.Balance,
                    Currency = a.Currency,
                    Status = a.Status,
                    DateOpened = a.DateOpened
                }).ToList();

                _logger.LogInformation("Successfully fetched {Count} accounts", response.Count);

                return ApiResponse<List<GetAllAccountsReponse>>
                    .SuccessResponse(response, "Accounts retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching accounts");
                return ApiResponse<List<GetAllAccountsReponse>>
                    .InternalServerError("An error occurred while retrieving accounts");

            }
        }
    }
}
