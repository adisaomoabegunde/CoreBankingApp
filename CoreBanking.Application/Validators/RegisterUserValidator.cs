using CoreBanking.Application.Commands.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username)
               .NotEmpty().WithMessage("Username is required")
               .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
               .MaximumLength(30).WithMessage("Username must not exceed 30 characters.")
               .Must(username => username != "string")
               .WithMessage("Username cannot be default value")
               .Matches("^[a-zA-Z0-9_]+$")
               .WithMessage("Username can only contain letters, numbers and underscores.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("Invalid email format")
               .Must(email => email != "string")
               .WithMessage("Email cannot be default value")
               .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be atleast 6 characters.")
                .MaximumLength(64).WithMessage("Password must not exceed 64 characters.")
               .Must(password => password != "string")
               .WithMessage("Password cannot be default value")
               .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
               .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
               .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
               .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");

        }
    }
}
