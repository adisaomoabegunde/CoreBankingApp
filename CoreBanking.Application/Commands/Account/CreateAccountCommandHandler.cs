using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreBanking.Application.Events;


namespace CoreBanking.Application.Commands.Account
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ApiResponse<string>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountNumberGenerator _accountNumberGenerator;
        private readonly ILogger<CreateAccountCommandHandler> _logger;
        private readonly IEventProducer _eventProducer;

        public CreateAccountCommandHandler(ICustomerRepository customerRepository, IAccountRepository accountRepository, ICurrentUserService currentUserService, IAccountNumberGenerator accountNumberGenerator, ILogger<CreateAccountCommandHandler> logger, IEventProducer eventProducer)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _currentUserService = currentUserService;
            _accountNumberGenerator = accountNumberGenerator;
            _logger = logger;
            _eventProducer = eventProducer;
        }
        
        public async Task<ApiResponse<string>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                _logger.LogInformation("Account creation started for user {userId}", userId);

                var customer = await _customerRepository.GetByUserIdAsync(userId);

                if(customer == null)
                {
                    _logger.LogWarning("Customer not found for user {userId}", userId);
                    return ApiResponse<string>.NotFound("Customer not found");
                }

                if(customer.KYCStatus != KycStatus.Verified)
                {
                    _logger.LogWarning("KYC not verified for customer {customerId}", customer.Id);
                    return ApiResponse<string>.BadRequest("KYC not verified");
                }
                var existingAccount = await _accountRepository
                    .GetByCustomerAndType(customer.Id, request.AccountType);

                if(existingAccount != null)
                {
                    _logger.LogWarning("Duplicate account attempt. CustomerId: {CustomerId}, Type: {AccountType}", customer.Id, request.AccountType);
                    return ApiResponse<string>.Duplicate("Account type already exists");
                }
                var accountNumber = await _accountNumberGenerator.GenerateAccountNumberAsync(request.AccountType);

                decimal minimumBalance = request.AccountType switch
                {
                    AccountType.Savings => 1000,
                    AccountType.Current => 5000,
                    AccountType.FixedDeposit => 10000,
                    _ => 0
                };

                if(request.InitialDeposit < minimumBalance)
                {
                    _logger.LogWarning("Insufficient opening balance. Required: {Min}, Provided: {Deposit}", minimumBalance, request.InitialDeposit);
                    return ApiResponse<string>.BadRequest($"Minimum opening balance is {minimumBalance}");
                }

                var account = new Domain.Entities.Account
                {
                    Id = Guid.NewGuid(),
                    AccountNumber = accountNumber,
                    CustomerId = customer.Id,
                    AccountType = request.AccountType,
                    Balance = request.InitialDeposit,
                    Currency = request.Currency,
                    Status = AccountStatus.Active,
                    DailyTransferlimit = 1_000_000,
                    MinimumBalance = minimumBalance,
                    InterestRate = request.AccountType == AccountType.Savings ? 0.02m : 0,
                    DateOpened = DateTime.UtcNow
                };

                await _accountRepository.AddAsync(account);

                await _eventProducer.PublishAsync("account.created", new AccountCreatedEvent
                {
                    AccountId = account.Id,
                    CustomerId = account.CustomerId,
                    AccountNumber = account.AccountNumber,
                    InitialBalance = account.Balance,
                    CreatedAt = DateTime.UtcNow
                });

                _logger.LogInformation("Account created successfully. AccountNumber: {AccountNumber}", accountNumber);

                return ApiResponse<string>
                    .SuccessResponse(accountNumber, "Account created successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during account creation");
                return ApiResponse<string>
                    .InternalServerError("Something went wrong");
            }
        }
    }
}
