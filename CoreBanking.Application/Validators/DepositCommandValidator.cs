using CoreBanking.Application.Commands.Transactions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class DepositCommandValidator : AbstractValidator<DepositCommand>
    {
        public DepositCommandValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .Length(10)
                .Matches("^[0-9]+$");
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(100)
                .LessThanOrEqualTo(10_000_000);
            RuleFor(x => x.IdempotencyKey)
                .NotEmpty();
        }
    }
}
