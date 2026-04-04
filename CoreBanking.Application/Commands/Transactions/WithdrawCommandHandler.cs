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

namespace CoreBanking.Application.Commands.Transactions
{
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, ApiResponse<string>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WithdrawCommandHandler> _logger;

        public WithdrawCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, ICustomerRepository customerRepository, ICurrentUserService currentUser,IAuditRepository auditRepository, IUnitOfWork unitOfWork, ILogger<WithdrawCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _customerRepository = customerRepository;
            _currentUser = currentUser;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            
        }
        public async Task<ApiResponse<string>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Withdrawal initiated for {Account}", request.AccountNumber);

                var existing = await _transactionRepository
                    .GetByIdempotencyKeyAsync(request.IdempotencyKey);
                if(existing != null)
                {
                    _logger.LogWarning("Duplicate transaction prevented: {Key}", request.IdempotencyKey);

                    return ApiResponse<string>
                        .SuccessResponse(existing.TransactionReference, "Duplicate request");
                }

                var account = await _accountRepository
                    .GetByAccountNumberAsync(request.AccountNumber);
                if (account == null)
                {
                    return ApiResponse<string>.NotFound("Account not found");
                }

                var userId = _currentUser.UserId;
                var customer = await _customerRepository.GetByUserIdAsync(userId);

                if(customer != null && account.CustomerId != customer.Id)
                {
                    return ApiResponse<string>.Unauthorized("You can only withdraw from your account");
                }

                if(account.Status != Domain.Enums.AccountStatus.Active)
                {
                    return ApiResponse<string>.BadRequest("Account is not active");
                }

                if (account.Balance - request.Amount < account.MinimumBalance)
                {
                    return ApiResponse<string>.BadRequest("Insufficient balance");
                }

                var todayTotal = await _transactionRepository.GetTodayWithdrawalTotalAsync(account.Id);

                if(todayTotal + request.Amount > 2_000_000)
                {
                    return ApiResponse<string>.BadRequest("Daily withdrawal limit exceeded");
                }

                var balanceBefore = account.Balance;

                account.Balance -= request.Amount;
                account.LastTransactionDate = DateTime.UtcNow;

                var reference = GenerateReference();

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TransactionReference = reference,
                    TransactionType = Domain.Enums.TransactionType.Withdrawal,
                    SourceAccountId = account.Id,
                    Amount = request.Amount,
                    BalanceAfter = account.Balance,
                    Status = Domain.Enums.TransactionStatus.Completed,
                    IdempotencyKey = request.IdempotencyKey,
                    ProcessedBy = userId,
                    TransactionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Description = request.Description
                };

                var ledger = new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    TransactionId = transaction.Id,
                    EntryType = Domain.Enums.EntryType.Debit,
                    Amount = request.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = account.Balance,
                    Description = "Withdrawal",
                    CreatedAt = DateTime.UtcNow
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.AddRangeAsync(new[] { ledger });

                await _transactionRepository.SaveChangesAsync();

                await _auditRepository.AddAsync(new AuditLog
                {
                    UserId = _currentUser.UserId,
                    Action = "Withdrawal",
                    IpAddress = _currentUser.IpAdress,
                    Description = $"Withdrew ₦{request.Amount} from account {account.AccountNumber}",
                    Timestamp = DateTime.UtcNow
                });


                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Withdrawal successfulll: {Reference}", reference);
                return ApiResponse<string>
                    .SuccessResponse(reference, "Withdrawal successful");
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(ex, "Withdrawal failed");


                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = _currentUser.UserId,
                    Action = "Withdrawal-Failed",
                    IpAddress = _currentUser.IpAdress ?? "Unknown",
                    Description = $"Failed Withdrawal attempt of ₦{request.Amount}  from {request.AccountNumber}",
                    Timestamp = DateTime.UtcNow
                });


                return ApiResponse<string>
                    .InternalServerError("Withdrawal failed");
            }
        }
        private string GenerateReference()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100000, 999999)}";
        }
    }
}
