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
    public class ReverseTransactionCommandHandler : IRequestHandler<ReverseTransactionCommand, ApiResponse<string>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReverseTransactionCommandHandler> _logger;

        public ReverseTransactionCommandHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository,ICurrentUserService currentUser, IAuditRepository auditRepository, IUnitOfWork unitOfWork, ILogger<ReverseTransactionCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _currentUser = currentUser;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ApiResponse<string>> Handle(ReverseTransactionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting transaction reversal for reference: {Reference}", request.Reference);

       
                var transaction = await _transactionRepository.GetByReferenceAsync(request.Reference);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction with reference {Reference} not found", request.Reference);
                    return ApiResponse<string>.NotFound("Transaction not found");
                }

                if (transaction.IsReversed)
                {
                    _logger.LogWarning("Transaction with reference {Reference} has already been reversed", request.Reference);
                    return ApiResponse<string>.BadRequest("Transaction has already been reversed");
                }

                if (transaction.Status != Domain.Enums.TransactionStatus.Completed)
                {
                    _logger.LogWarning("Transaction with reference {Reference} is not completed and cannot be reversed", request.Reference);
                    return ApiResponse<string>.BadRequest("Only completed transactions can be reversed");
                }

                var reversalRef = GenerateReference();

                var  IdempotencyKey = $"reverse-{transaction.TransactionReference}";

                var reversalTransaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    TransactionReference = reversalRef,
                    TransactionType = TransactionType.Reversal,
                    Amount = transaction.Amount,
                    Status = TransactionStatus.Completed,
                    ReversedTransactionId = transaction.Id,
                    Description = $"Reversal of transaction {transaction.TransactionReference}",
                    IdempotencyKey = IdempotencyKey,
                    TransactionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                var reversalId = reversalTransaction.Id;
                await _transactionRepository.AddAsync(reversalTransaction);

                var ledgerEntries = new List<LedgerEntry>();

                switch (transaction.TransactionType)
                {
                    case Domain.Enums.TransactionType.Transfer:
                        await ReverseTransfer(transaction, reversalTransaction, reversalRef, ledgerEntries);
                        break;

                    case Domain.Enums.TransactionType.Deposit:
                        await ReverseDeposit(transaction, reversalTransaction, reversalRef, ledgerEntries);
                        break;

                    case Domain.Enums.TransactionType.Withdrawal:
                        await ReverseWithdrawal(transaction, reversalTransaction, reversalRef, ledgerEntries);
                        break;

                    default:
                        throw new Exception("Unsupported transaction type for reversal");
                }

                transaction.IsReversed = true;
                transaction.Status = Domain.Enums.TransactionStatus.Reversed;

                await _transactionRepository.AddRangeAsync(ledgerEntries);
                await _transactionRepository.SaveChangesAsync();

                await _auditRepository.AddAsync(new AuditLog
                {
                    UserId = _currentUser.UserId,
                    Action = "Reversal",
                    IpAddress = _currentUser.IpAdress,
                    Description = $"Reversed transaction {request.Reference}",
                    Timestamp = DateTime.UtcNow
                });

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Transaction reversal successful for reference: {Reference}", reversalRef);
                return ApiResponse<string>.SuccessResponse(reversalRef, "Transaction reversed successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error occurred while reversing transaction with reference: {Reference}", request.Reference);

                await _auditRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = _currentUser.UserId,
                    Action = "Reversal-Failed",
                    IpAddress = _currentUser.IpAdress ?? "Unknown",
                    Description = $"Failed Reversal attempt of  {request.Reference}",
                    Timestamp = DateTime.UtcNow
                });

                return ApiResponse<string>.InternalServerError( ex.InnerException?.Message ?? ex.Message);
            }
        }


        private async Task ReverseTransfer(Transaction original, Transaction reversalTransaction, string refNo, List<LedgerEntry> ledgerEntries)
        {
            var source = await _accountRepository.GetByIdAsync(original.SourceAccountId.Value);
            var destination = await _accountRepository.GetByIdAsync(original.DestinationAccountId.Value);

            if (source == null || destination == null)
            {
                throw new Exception("Source or destination account not found for reversal");
            }

            var sourceBefore = source.Balance;
            var destinationBefore = destination.Balance;

           

            source.Balance += original.Amount;
            destination.Balance -= original.Amount;

            ledgerEntries.Add(new LedgerEntry
            {
                Id = Guid.NewGuid(),
                AccountId = source.Id,
                TransactionId = reversalTransaction.Id,
                EntryType = Domain.Enums.EntryType.Credit,
                Amount = original.Amount,
                BalanceBefore = sourceBefore,
                BalanceAfter = source.Balance,
                Description = "Transfer Reversal",
                CreatedAt = DateTime.UtcNow
            });
            ledgerEntries.Add(new LedgerEntry
            {
                Id = Guid.NewGuid(),
                AccountId = destination.Id,
                TransactionId = reversalTransaction.Id,
                EntryType = Domain.Enums.EntryType.Debit,
                Amount = original.Amount,
                BalanceBefore = destinationBefore,
                BalanceAfter = destination.Balance,
                Description = "Transfer Reversal",
                CreatedAt = DateTime.UtcNow
            });
        }

        private async Task ReverseDeposit(
        Transaction original,Transaction reversalTransaction,
        string refNo,
        List<LedgerEntry> ledgerEntries)
        {
            var account = await _accountRepository.GetByIdAsync(original.DestinationAccountId.Value);
            var before = account.Balance;
            account.Balance -= original.Amount;

            ledgerEntries.Add(new LedgerEntry
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                TransactionId = reversalTransaction.Id,
                EntryType = EntryType.Debit,
                Amount = original.Amount,
                BalanceBefore = before,
                BalanceAfter = account.Balance,
                Description = "Deposit Reversal",
                CreatedAt = DateTime.UtcNow
            });
        }

        private async Task ReverseWithdrawal(
            Transaction original, Transaction reversalTransaction,
            string refNo,
            List<LedgerEntry> ledgerEntries)
        {
            var account = await _accountRepository.GetByIdAsync(original.SourceAccountId.Value);
            var before = account.Balance;


            account.Balance += original.Amount;

            ledgerEntries.Add(new LedgerEntry
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                TransactionId = reversalTransaction.Id,
                EntryType = EntryType.Credit,
                Amount = original.Amount,
                BalanceBefore = before,
                BalanceAfter = account.Balance,
                Description = "Withdrawal Reversal",
                CreatedAt = DateTime.UtcNow
            });
        }

        private string GenerateReference()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100000, 999999)}";
        }
    }
}

