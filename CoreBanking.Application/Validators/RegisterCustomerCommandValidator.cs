using CoreBanking.Application.Commands.Customers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50);
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^0[0-9]{10}$")
                .WithMessage("Phone number must be a valid Nigerian number (11 digits)");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200);
            RuleFor(x => x.BVN)
                .NotEmpty().WithMessage("BVN is required")
                .Length(11).WithMessage("BVN must be exactly 11 digits")
                .Matches(@"^[0-9]{11}$")
                .WithMessage("BVN must contain only numbers");
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(dob => BeAtLeast18YearsOld(dob))
                .WithMessage("Customer must be at least 18 years old");
        }
        private bool BeAtLeast18YearsOld(DateTime dob)
        {
            return dob <= DateTime.Today.AddYears(-16);
        }
    }
}
