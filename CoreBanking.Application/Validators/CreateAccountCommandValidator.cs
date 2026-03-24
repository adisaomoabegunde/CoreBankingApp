using CoreBanking.Application.Commands.Account;
using CoreBanking.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.AccountType)
                .IsInEnum()
                .WithMessage("Invalid account type");
            RuleFor(x => x.Currency)
                .IsInEnum()
                .WithMessage("Invalid currency");
            RuleFor(x => x.InitialDeposit)
                .GreaterThan(0)
                .WithMessage("Initial deposit must be greater than zero")
                .LessThanOrEqualTo(100000000)
                .WithMessage("initial deposit exceeds allowed limit");


            // Savings Rule
            When(x => x.AccountType == AccountType.Savings, () =>
            {
                RuleFor(x => x.InitialDeposit)
                    .GreaterThanOrEqualTo(1000)
                    .WithMessage("Minimum opeming balance for savings is N1,000");
            });

            //Current Rule
            When(x => x.AccountType == AccountType.Current, () =>
            {
                RuleFor(x => x.InitialDeposit)
                    .GreaterThanOrEqualTo(5000)
                    .WithMessage("Minimum opening balance for Current is N5,000");

            });

            // Fixed Deposit Rule 
            When(x => x.AccountType == AccountType.FixedDeposit, () =>
            {
                RuleFor(x => x.InitialDeposit)
                    .GreaterThanOrEqualTo(10000)
                    .WithMessage("Minimum for Fixed Deposit is N10,000");
            });
        }
    }
}
