using CoreBanking.Application.Commands.Customers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^[0-9]+$")
                .WithMessage("Phone number must contain only numbers");
            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required");
            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today)
                .WithMessage("Invalid date of birth");
        }
    }
}
