using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace CoreBanking.Application.Commands.Account
{
    public class ChangeAccountStatusCommandHandler : IRequestHandler<ChangeAccountStatusCommand, ApiResponse<string>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<ChangeAccountStatusCommandHandler> _logger;
        private readonly IEventProducer _eventProducer;

        public ChangeAccountStatusCommandHandler(IAccountRepository accountRepository, ILogger<ChangeAccountStatusCommandHandler> logger, IEventProducer eventProducer)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _eventProducer = eventProducer;
        }

        public async Task<ApiResponse<string>> Handle(ChangeAccountStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Changing account status. AccountNumber: {AccountNumber}, NewStatus: {NewStatus}", request.AccountNumber, request.Status);

                var account = await _accountRepository.GetByAccountNumberAsync(request.AccountNumber);
                if (account == null)
                {
                    _logger.LogWarning("Account not found. AccountNumber: {AccountNumber}", request.AccountNumber);
                    return ApiResponse<string>.NotFound("Account not found");
                }

                if (account.Status == AccountStatus.Closed)
                {
                    _logger.LogWarning("Attempt to change status of a closed account. AccountNumber: {AccountNumber}", request.AccountNumber);
                    return ApiResponse<string>.BadRequest("Cannot change status of a closed account");
                }

                if (account.Status == request.Status)
                {
                    _logger.LogInformation("Account already in the desired status. AccountNumber: {AccountNumber}, Status: {Status}", request.AccountNumber, request.Status);
                    return ApiResponse<string>.BadRequest("Account is already in the desired status");
                }
                if (request.Status == AccountStatus.Closed && account.Balance > 0)
                {
                    _logger.LogWarning("Attempt to close an account with a positive balance. AccountNumber: {AccountNumber}, Balance: {Balance}", request.AccountNumber, account.Balance);
                    return ApiResponse<string>.BadRequest("Cannot close an account with a positive balance");
                }

                account.Status = request.Status;

                await _accountRepository.UpdateAsync(account);

                try
                {
                    await _eventProducer.PublishAsync("account.status.changed", new AccountStatusChangedEvent
                    {
                        AccountId = account.Id,
                        AccountNumber = account.AccountNumber,
                        OldStatus = account.Status,
                        NewStatus = request.Status,
                        Reference = $"Status-{DateTime.UtcNow.Ticks}"

                    });

                    _logger.LogInformation("Published account status changed event. AccountNumber: {AccountNumber}, NewStatus: {NewStatus}", request.AccountNumber, request.Status);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish account status changed event. AccountNumber: {AccountNumber}", request.AccountNumber);
                }


                    _logger.LogInformation("Account status changed successfully. AccountNumber: {AccountNumber}, NewStatus: {NewStatus}", request.AccountNumber, request.Status);
                return ApiResponse<string>
                    .SuccessResponse($"Account status changed to {request.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing account status. AccountNumber: {AccountNumber}", request.AccountNumber);
                return ApiResponse<string>.InternalServerError("An error occurred while changing the account status");
            }
        }
    }
}
