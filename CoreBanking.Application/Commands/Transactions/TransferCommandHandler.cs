using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Transactions
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, ApiResponse<string>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransferCommandHandler> _logger;

        public TransferCommandHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ILogger<TransferCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Initiating transfer from {Source} to {Description}",
                    request.SourceAccountNumber, request.DestinationAccountNumber);

                var existing = await _transactionRepository
                    .GetByIdempotencyKeyAsync(request.IdempotencyKey);

                if(existing != null)
                {
                    _logger.LogWarning("Duplicate transaction prevented: {Key}", request.IdempotencyKey);
                    return ApiResponse<string>.SuccessResponse(existing.TransactionReference, "Duplicate request");
                }

                var source = await _accountRepository.GetByAccountNumberAsync(request.SourceAccountNumber);
                var destination = await _accountRepository.GetByAccountNumberAsync(request.DestinationAccountNumber);

                if (source == null || destination == null)
                {
                    return ApiResponse<string>.NotFound("Invalid account");
                }
                if(source.Status != AccountStatus.Active || destination.Status != AccountStatus.Active)
                {
                    return ApiResponse<string>.BadRequest("Account not active");
                }
                if (source.Balance - request.Amount < source.MinimumBalance)
                {
                    return ApiResponse<string>.BadRequest("Insufficient balance");
                }

                var todayTotal = await _transactionRepository.GetTodayTransferTotalAsync(source.Id);

                if(todayTotal + request.Amount > source.DailyTransferlimit)
                {
                    return ApiResponse<string>.BadRequest("Daily transfer limit exceeded");
                }

                var todayCount = await _transactionRepository.GetTodayTransferCountAsync(source.Id);

                if(todayCount >= 10)
                {
                    return ApiResponse<string>.BadRequest("Max transfer per day reached");
                }

                var reference = GenerateReference();

                var sourceBefore = source.Balance;
                var destBefore = destination.Balance;

                source.Balance -= request.Amount;
                destination.Balance += request.Amount;

                source.LastTransactionDate = DateTime.UtcNow;
                destination.LastTransactionDate = DateTime.UtcNow;

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TransactionReference = reference,
                    TransactionType = TransactionType.Transfer,
                    SourceAccountId = source.Id,
                    DestinationAccountId = destination.Id,
                    Amount = request.Amount,
                    Status = TransactionStatus.Completed,
                    IdempotencyKey = request.IdempotencyKey,
                    TransactionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Description = request.Description
                };

                var debitEntry = new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    AccountId = source.Id,
                    TransactionId = transaction.Id,
                    EntryType = EntryType.Debit,
                    Amount = request.Amount,
                    BalanceBefore = sourceBefore,
                    BalanceAfter = source.Balance,
                    Description = $"Transfer to {destination.AccountNumber}",
                    CreatedAt = DateTime.UtcNow
                };

                var creditEntry = new LedgerEntry
                {
                    Id = Guid.NewGuid(),
                    AccountId = destination.Id,
                    TransactionId = transaction.Id,
                    EntryType = EntryType.Credit,
                    Amount = request.Amount,
                    BalanceBefore = destBefore,
                    BalanceAfter = destination.Balance,
                    Description = $"Transfer from {source.AccountNumber}",
                    CreatedAt = DateTime.UtcNow
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.AddRangeAsync(new[] { debitEntry, creditEntry });
                
                await _transactionRepository.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Transfer successful: {Reference}", reference);
                return ApiResponse<string>.SuccessResponse(reference, "Transfer successful");

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Transfer failed");

                return ApiResponse<string>.InternalServerError("Transfer failed");

            }
        }
        private string GenerateReference()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100000, 999999)}";
        }
    }
}
