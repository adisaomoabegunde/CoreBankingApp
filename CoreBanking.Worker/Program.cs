using CoreBanking.Infrastructure;
using CoreBanking.Infrastructure.Persistence;
using CoreBanking.Worker;
using CoreBanking.Worker.Consumers;
using CoreBanking.Worker.Services;
using CoreBanking.Worker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddInfrastructureDI(builder.Configuration);

builder.Services.AddSingleton<TransactionConsumer>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddScoped<ITransferCacheService, TransferCacheService>();
builder.Services.AddScoped<ITransferAuditService, TransferAuditService>();
builder.Services.AddScoped<ITransferNotificationService, TransferNotificationService>();
builder.Services.AddScoped<ITransferFraudService, TransferFraudService>();

builder.Services.AddScoped<IDepositCacheService, DepositCacheService>();
builder.Services.AddScoped<IDepositNotificationService, DepositNotificationService>();
builder.Services.AddScoped<IDepositAuditService, DepositAuditService>();
builder.Services.AddScoped<IDepositFraudService, DepositFraudService>();

builder.Services.AddScoped<IWithdrawalCacheService, WithdrawalCacheService>();
builder.Services.AddScoped<IWithdrawalNotificationService, WithdrawalNotificationService>();
builder.Services.AddScoped<IWithdrawalAuditService, WithdrawalAuditService>();
builder.Services.AddScoped<IWithdrawalFraudService, WithdrawalFraudService>();

builder.Services.AddScoped<IReversalCacheService, ReversalCacheService>();
builder.Services.AddScoped<IReversalNotificationService, ReversalNotificationService>();
builder.Services.AddScoped<IReversalAuditService, ReversalAuditService>();

builder.Services.AddScoped<IAccountCacheService, AccountCacheService>();
builder.Services.AddScoped<IAccountNotificationService, AccountNotificationService>();
builder.Services.AddScoped<IAccountAuditService, AccountAuditService>();

builder.Services.AddScoped<IAccountStatusCacheService, AccountStatusCacheService>();
builder.Services.AddScoped<IAccountStatusNotificationService, AccountStatusNotificationService>();
builder.Services.AddScoped<IAccountStatusAuditService, AccountStatusAuditService>();

builder.Services.AddScoped<ICustomerCacheService, CustomerCacheService>();
builder.Services.AddScoped<ICustomerNotificationService, CustomerNotificationService>();
builder.Services.AddScoped<ICustomerAuditService, CustomerAuditService>();

builder.Services.AddScoped<ICustomerKycCacheService, CustomerKycCacheService>();
builder.Services.AddScoped<ICustomerKycnotificationService, CustomerKycNotificationService>();
builder.Services.AddScoped<ICustomerKycAuditService, CustomerKycAuditService>();

builder.Services.AddScoped<IUserNotificationService, UserNotificationServiceb>();
builder.Services.AddScoped<IUserAuditService, UserAuditService>();

builder.Services.AddScoped<IUserLoginCacheService, UserLoginCacheService>();
builder.Services.AddScoped<IUserLoginAuditService, UserLoginAuditService>();
builder.Services.AddScoped<IUserSecurityService, UserSecurityService>();

builder.Services.AddScoped<IUserLogoutCacheService, UserLogoutCacheService>();
builder.Services.AddScoped<IUserLogoutAuditService, UserLogoutAuditService>();

var host = builder.Build();
host.Run();
