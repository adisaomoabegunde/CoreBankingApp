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
    public class DepositCommandHandler : IRequestHandler<DepositCommand, ApiResponse<string>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<DepositCommandHandler> _logger;

        public DepositCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUser,IAuditRepository auditRepository, ILogger<DepositCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Deposit initiated for {Account}", request.AccountNumber);

                var existing = await _transactionRepository.GetByIdempotencyKeyAsync(request.IdempotencyKey);

                if (existing != null)
                {
                    _logger.LogWarning("Duplicate transaction prevented: {Key}", request.IdempotencyKey);
                    return ApiResponse<string>
                        .SuccessResponse(existing.TransactionReference, "Duplicate request");
                }

                var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber);

                if (account == null)
                {
                    return ApiResponse<string>.NotFound("Account not found");

                }

                if(account.Status != Domain.Enums.AccountStatus.Active)
                {
                    return ApiResponse<string>.BadRequest("Account is not active");

                }

                var balancebefore = account.Balance;

                account.Balance += request.Amount;
                account.LastTransactionDate = DateTime.UtcNow;

                var reference = GenerateReference();

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TransactionReference = reference,
                    TransactionType = Domain.Enums.TransactionType.Deposit,
                    DestinationAccountId = account.Id,
                    Amount = request.Amount,
                    BalanceAfter = account.Balance,
                    Status = Domain.Enums.TransactionStatus.Completed,
                    IdempotencyKey = request.IdempotencyKey,
                    ProcessedBy = _currentUser.UserId,
                    TransactionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Description = request.Description
                };

                var ledger = new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    TransactionId = transaction.Id,
                    EntryType = Domain.Enums.EntryType.Credit,
                    Amount = request.Amount,
                    BalanceBefore = balancebefore,
                    BalanceAfter = account.Balance,
                    Description = "Deposit",
                    CreatedAt = DateTime.UtcNow
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.AddRangeAsync(new[] { ledger });

                await _transactionRepository.SaveChangesAsync();


                await _auditRepository.AddAsync(new AuditLog
                {
                    UserId = _currentUser.UserId,
                    Action = "Deposit",
                    IpAddress = _currentUser.IpAdress,
                    Description = $"Deposited ₦{request.Amount} into account {account.AccountNumber}",
                    Timestamp = DateTime.UtcNow
                });

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Deposit successfull: {Reference}", reference);

                return ApiResponse<string>
                    .SuccessResponse(reference, "Deposit successful");

            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(ex, "Deposit failed");

                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = _currentUser.UserId,
                    Action = "Deposit-Failed",
                    IpAddress = _currentUser.IpAdress ?? "Unknown",
                    Description = $"Failed Deposit attempt of ₦{request.Amount}  to {request.AccountNumber}",
                    Timestamp = DateTime.UtcNow
                });

                return ApiResponse<string>
                    .InternalServerError("Deposit failed");
            }

        }

        private string GenerateReference()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100000, 999999)}";
        }
    }
}
