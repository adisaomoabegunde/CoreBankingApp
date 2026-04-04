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
    public class GetTransactionByReferenceQueryHandler : IRequestHandler<GetTransactionByReferenceQuery, ApiResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<GetAccountTransactionQueryHandler> _logger;

        public GetTransactionByReferenceQueryHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICustomerRepository customerRepository, ICurrentUserService currentUser, ILogger<GetAccountTransactionQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ApiResponse<TransactionDto>> Handle(GetTransactionByReferenceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching transaction {Reference}", request.Reference);

                var transaction = await _transactionRepository.GetByReferenceAsync(request.Reference);
                if(transaction == null)
                {
                    return ApiResponse<TransactionDto>.NotFound("Transaction not found");
                }

                var userId = _currentUser.UserId;
                var customer = await _customerRepository.GetByUserIdAsync(userId);

                if(customer != null)
                {
                    var isOwner = transaction.SourceAccountId != null && await _accountRepository.IsOwnedByCustomer(transaction.SourceAccountId.Value, customer.Id) ||
                        transaction.DestinationAccountId != null && await _accountRepository.IsOwnedByCustomer(transaction.DestinationAccountId.Value, customer.Id);

                    if (!isOwner)
                    {
                        return ApiResponse<TransactionDto>
                            .Unauthorized("You are not allowed to view this transaction");
                    }


                }

                string sourceAccount = null;
                string destinationAccount = null;

                if(transaction.SourceAccountId != null)
                {
                    var src = await _accountRepository.GetByIdAsync(transaction.SourceAccountId.Value);
                    sourceAccount = src?.AccountNumber;
                }

                if(transaction.DestinationAccountId != null)
                {
                    var dest = await _accountRepository.GetByIdAsync(transaction.DestinationAccountId.Value);
                    destinationAccount = dest?.AccountNumber;
                }

                var response = new TransactionDto
                {
                    TransactionReference = transaction.TransactionReference,
                    TransactionType = transaction.TransactionType.ToString(),
                    Amount = transaction.Amount,
                    Status = transaction.Status.ToString(),
                    SourceAccountNumber = sourceAccount,
                    DestinationAccountNumber = destinationAccount,
                    TransactionDate = transaction.TransactionDate,
                    Description = transaction.Description
                };

                _logger.LogInformation("Transaction retrieved successfully");
                return ApiResponse<TransactionDto>
                    .SuccessResponse(response, "Transaction retrieved");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching transaction for reference {Reference}", request.Reference);
                return ApiResponse<TransactionDto>
                    .InternalServerError("Failed to retrive transaction" + ex.Message);

            }
        }
    }
}
