using CoreBanking.Application.Commands.Account;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class ChangeAccountStatusCommandValidator : AbstractValidator<ChangeAccountStatusCommand>
    {
        public ChangeAccountStatusCommandValidator()
        {
            
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid account status.");
        }
    }
}
