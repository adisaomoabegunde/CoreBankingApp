using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Customers
{
    public class UpdateCustomerKycStatusCommandHandler : IRequestHandler<UpdateCustomerKycStatusCommand, ApiResponse<string>>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerKycStatusCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ApiResponse<string>> Handle(UpdateCustomerKycStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(request.Id);
                if(customer == null)
                {
                    return ApiResponse<string>.NotFound("Customer not found");
                }
                customer.KYCStatus = request.KycStatus;
                customer.UpdatedAt = DateTime.UtcNow;

                await _customerRepository.UpdateAsync(customer);

                return ApiResponse<string>
                    .SuccessResponse("Customer KYC status updated successfully");
            }
            catch(Exception ex)
            {
                return ApiResponse<string>
                    .InternalServerError("Failed to update KYC status: " + ex.Message);
            }
        }
    }
}
