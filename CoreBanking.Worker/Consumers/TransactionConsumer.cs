using Confluent.Kafka;
using CoreBanking.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Enums;
using CoreBanking.Worker.Services.Interfaces;


namespace CoreBanking.Worker.Consumers
{
    public class TransactionConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransactionConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;


        public TransactionConsumer(ILogger<TransactionConsumer> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "corebanking-worker-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe(new[]
            {
                "transfer.completed",
                "deposit.completed" ,
                "withdrawal.completed",
                "transaction.reversed",
                "account.created",
                "account.status.changed",
                "customer.registered",
                "customer.kyc.updated",
                "user.registered",
                "user.loggedin",
                "user.loggedout"
            });

            _logger.LogInformation("Transaction consumer started....");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);

                    if (result.Topic == "transfer.completed")
                    {
                        var transferEvent = JsonSerializer.Deserialize<TransferCompletedEvent>(result.Message.Value);

                        if (transferEvent != null)
                        {
                            await ProcessTransactionEvent(transferEvent);
                            _logger.LogInformation("Transaction event processed: {Reference}", transferEvent.Reference);

                        }

                    }

                    if (result.Topic == "deposit.completed")
                    {
                        var depositEvent = JsonSerializer.Deserialize<DepositCompletedEvent>(result.Message.Value);
                        if (depositEvent != null)
                        {
                            await ProcessDepositCompleted(depositEvent);
                            _logger.LogInformation("Deposit event processed: {Reference}", depositEvent.Reference);
                        }
                    }

                    if (result.Topic == "withdrawal.completed")
                    {
                        var withdrawalEvent = JsonSerializer.Deserialize<WithdrawalCompletedEvent>(result.Message.Value);
                        if (withdrawalEvent != null)
                        {
                            await ProcessWithdrawalCompleted(withdrawalEvent);
                            _logger.LogInformation("Withdrawal event processed: {Reference}", withdrawalEvent.Reference);
                        }
                    }

                    if (result.Topic == "transaction.reversed")
                    {
                        var reversalEvent = JsonSerializer.Deserialize<TransactionReversedEvent>(result.Message.Value);
                        if (reversalEvent != null)
                        {
                            await ProcessTransactionReversal(reversalEvent);
                            _logger.LogInformation("Transaction reversal event processed: {Reference}", reversalEvent.Reference);
                        }
                    }

                    if(result.Topic == "account.created")
                    {
                        var accountEvent = JsonSerializer.Deserialize<AccountCreatedEvent>(result.Message.Value);

                        if (accountEvent != null)
                        {
                            await ProcessAccountCreated(accountEvent);
                        }
                    }

                    if(result.Topic == "account.status.changed")
                    {
                        var statusEvent = JsonSerializer.Deserialize<AccountStatusChangedEvent>(result.Message.Value);

                        if(statusEvent != null)
                        {
                            await ProcessAccountStatusChanged(statusEvent);
                        }
                    }

                    if(result.Topic == "customer.registered")
                    {
                        var customerEvent = JsonSerializer.Deserialize<CustomerRegisteredEvent>(result.Message.Value);

                        if(customerEvent != null)
                        {
                            await ProcessCustomerRegistered(customerEvent);
                        }
                    }
                    if(result.Topic == "customer.kyc.updated")
                    {
                        var kycEvent = JsonSerializer.Deserialize<CustomerKycUpdatedEvent>(result.Message.Value);

                        if(kycEvent != null)
                        {
                            await ProcessCustomerKycUpdated(kycEvent);
                        }
                    }

                    if(result.Topic == "user.registered")
                    {
                        var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(result.Message.Value);

                        if(userEvent != null)
                        {
                            await ProcessUserRegisteredEvent(userEvent);
                        }
                    }
                    if(result.Topic == "user.loggedin")
                    {
                        var loginEvent = JsonSerializer.Deserialize<UserLoggedInEvent>(result.Message.Value);
                        if(loginEvent != null)
                        {
                            await ProcessUserLogin(loginEvent);
                        }
                    }
                    if(result.Topic == "user.loggedout")
                    {
                        var logoutEvent = JsonSerializer.Deserialize<UserLoggedOutEvent>(result.Message.Value);
                        if(logoutEvent != null)
                        {
                            await ProcessUserLogout(logoutEvent);
                        }
                    }

                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming transaction event");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing transfer event");
                }

            }
        }

        private async Task ProcessTransactionEvent(TransferCompletedEvent transferEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cacheService = scope.ServiceProvider.GetRequiredService<ITransferCacheService>();
                var auditService = scope.ServiceProvider.GetRequiredService<ITransferAuditService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<ITransferNotificationService>();
                var fraudService = scope.ServiceProvider.GetRequiredService<ITransferFraudService>();

                await cacheService.HandleAsync(transferEvent);
                await auditService.HandleAsync(transferEvent);
                await notificationService.HandleAsync(transferEvent);
                await fraudService.HandleAsync(transferEvent);

                _logger.LogInformation(
                "Transaction completed: Ref={Reference}, Amount={Amount}",
                transferEvent.Reference,
                transferEvent.Amount
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction event for {Reference}", transferEvent.Reference);


            }
        }
        private async Task ProcessDepositCompleted(DepositCompletedEvent depositEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cacheService = scope.ServiceProvider.GetRequiredService<IDepositCacheService>();
                var auditService = scope.ServiceProvider.GetRequiredService<IDepositAuditService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<IDepositNotificationService>();
                var fraudService = scope.ServiceProvider.GetRequiredService<IDepositFraudService>();

                await cacheService.HandleAsync(depositEvent);
                await auditService.HandleAsync(depositEvent);
                await notificationService.HandleAsync(depositEvent);
                await fraudService.HandleAsync(depositEvent);

                _logger.LogInformation("Deposit completed: Ref={Reference}, Amount={Amount}", depositEvent.Reference, depositEvent.Amount);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deposit event for {Reference}", depositEvent.Reference);
            }


        }

        private async Task ProcessWithdrawalCompleted(WithdrawalCompletedEvent withdrawalEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<IWithdrawalCacheService>();
                var auditService = scope.ServiceProvider.GetRequiredService<IWithdrawalAuditService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<IWithdrawalNotificationService>();
                var fraudService = scope.ServiceProvider.GetRequiredService<IWithdrawalFraudService>();

                await cacheService.HandleAsync(withdrawalEvent);
                await auditService.HandleAsync(withdrawalEvent);
                await notificationService.HandleAsync(withdrawalEvent);
                await fraudService.HandleAsync(withdrawalEvent);
                _logger.LogInformation("Withdrawal completed: Ref={Reference}, Amount={Amount}", withdrawalEvent.Reference, withdrawalEvent.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal event for {Reference}", withdrawalEvent.Reference);
            }
        }

        private async Task ProcessTransactionReversal(TransactionReversedEvent reversalEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<IReversalCacheService>();
                var auditService = scope.ServiceProvider.GetRequiredService<IReversalAuditService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<IReversalNotificationService>();

                await cacheService.HandleAsync(reversalEvent);
                await auditService.HandleAsync(reversalEvent);
                await notificationService.HandleAsync(reversalEvent);
                _logger.LogInformation("Transaction reversal processed: Ref={Reference}, Amount={Amount}", reversalEvent.Reference, reversalEvent.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction reversal for {Reference}", reversalEvent.Reference);
            }
        }
        private async Task ProcessAccountCreated(AccountCreatedEvent accountCreatedEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cacheService = scope.ServiceProvider.GetRequiredService<IAccountCacheService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<IAccountNotificationService>();
                var auditService = scope.ServiceProvider.GetRequiredService<IAccountAuditService>();

                await cacheService.HandleAsync(accountCreatedEvent);
                await notificationService.HandleAsync(accountCreatedEvent);
                await auditService.HandleAsync(accountCreatedEvent);

                _logger.LogInformation("Account created processsed: {AccountNumber}", accountCreatedEvent.AccountNumber);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error creating account for {Account}", accountCreatedEvent.AccountNumber);
            }
        }

        private async Task ProcessAccountStatusChanged(AccountStatusChangedEvent accountStatusChangedEvent)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var cacheService = scope.ServiceProvider.GetRequiredService<IAccountStatusCacheService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<IAccountStatusNotificationService>();
                var auditService = scope.ServiceProvider.GetRequiredService<IAccountStatusAuditService>();

                await cacheService.HandleAsync(accountStatusChangedEvent);
                await notificationService.HandleAsync(accountStatusChangedEvent);
                await auditService.HandleAsync(accountStatusChangedEvent);

                _logger.LogInformation("Account status changed processed: {AccountNumber}", accountStatusChangedEvent.AccountNumber);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error changing account status for {AccountNumber}", accountStatusChangedEvent.AccountNumber);
            }
        }

        private async Task ProcessCustomerRegistered(CustomerRegisteredEvent registeredEvent)
        {
            using var scope = _scopeFactory.CreateScope();

            var cacheService = scope.ServiceProvider.GetRequiredService<ICustomerCacheService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<ICustomerNotificationService>();
            var auditService = scope.ServiceProvider.GetRequiredService<ICustomerAuditService>();

            await cacheService.HandleAsync(registeredEvent);
            await notificationService.HandleAsync(registeredEvent);
            await auditService.HandleAsync(registeredEvent);

        }

        private async Task ProcessCustomerKycUpdated(CustomerKycUpdatedEvent kycUpdatedEvent)
        {
            using var scope = _scopeFactory.CreateScope();

            var cacheService = scope.ServiceProvider.GetRequiredService<ICustomerKycCacheService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<ICustomerKycnotificationService>();
            var auditService = scope.ServiceProvider.GetRequiredService<ICustomerKycAuditService>();

            await cacheService.HandleAsync(kycUpdatedEvent);
            await notificationService.HandleAsync(kycUpdatedEvent);
            await auditService.HandleAsync(kycUpdatedEvent);
        }

        private async Task ProcessUserRegisteredEvent(UserRegisteredEvent userEvent)
        {
            using var scope = _scopeFactory.CreateScope();

            var notificationService = scope.ServiceProvider.GetRequiredService<IUserNotificationService>();
            var auditService = scope.ServiceProvider.GetRequiredService<IUserAuditService>();

            await notificationService.HandleAsync(userEvent);
            await auditService.HandleAsync(userEvent);  
        }

        private async Task ProcessUserLogin(UserLoggedInEvent loggedInEvent)
        {
            using var scope = _scopeFactory.CreateScope();

            var cacheService = scope.ServiceProvider.GetRequiredService<IUserLoginCacheService>();
            var auditService = scope.ServiceProvider.GetRequiredService<IUserLoginAuditService>();
            var securityService = scope.ServiceProvider.GetRequiredService<IUserSecurityService>();

            await cacheService.HandleAsync(loggedInEvent);
            await auditService.HandleAsync(loggedInEvent);
            await securityService.HandleAsync(loggedInEvent);
        }

        private async Task ProcessUserLogout(UserLoggedOutEvent loggedOutEvent)
        {
            using var scope = _scopeFactory.CreateScope();

            var cacheService = scope.ServiceProvider.GetRequiredService<IUserLogoutCacheService>();
            var auditService = scope.ServiceProvider.GetRequiredService<IUserLogoutAuditService>();

            await cacheService.HandleAsync(loggedOutEvent);
            await auditService.HandleAsync(loggedOutEvent);
        }
    }
}
