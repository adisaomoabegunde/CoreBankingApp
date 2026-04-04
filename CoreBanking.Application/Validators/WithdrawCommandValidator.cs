using CoreBanking.Application.Commands.Transactions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
    {
        public WithdrawCommandValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .Length(10)
                .Matches("^[0-9]+$");
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(100)
                .LessThanOrEqualTo(500_000);
            RuleFor(x => x.IdempotencyKey)
                .NotEmpty();
        }
    }
}
