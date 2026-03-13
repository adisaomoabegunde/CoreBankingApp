using CoreBanking.Application.Commands.Customers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class UpdateCustomerKycStatusCommandValidator : AbstractValidator<UpdateCustomerKycStatusCommand>
    {
        public UpdateCustomerKycStatusCommandValidator()
        {
            RuleFor(x => x.KycStatus)
                .NotEmpty()
                .Must(x => x == "Pending" || x == "Verified" || x == "Rejected")
                .WithMessage("KYC status must be Pending, Verified or Rejected");
        }
    }
}
