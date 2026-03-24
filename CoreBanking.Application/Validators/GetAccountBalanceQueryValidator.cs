using CoreBanking.Application.Queries.Accounts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class GetAccountBalanceQueryValidator : AbstractValidator<GetAccountBalanceQuery>
    {
        public GetAccountBalanceQueryValidator()
        {
            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .Length(10)
                .Matches("^[0-9]+$")
                .WithMessage("Account number must be a 10-digit numeric string.");
        }
    }
}
