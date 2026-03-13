using CoreBanking.Application.DTOs;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Queries.Customers
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, ApiResponse<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<ApiResponse<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(request.Id);
                if(customer == null)
                {
                    return ApiResponse<CustomerDto>
                        .NotFound("customer not found");
                }
                var result = new CustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    DateOfBirth = customer.DateOfBirth,
                    Address = customer.Address,
                    BVN = customer.BVN,
                    KYCStatus = customer.KYCStatus,
                    CreatedAt = customer.CreatedAt
                };
                return ApiResponse<CustomerDto>
                    .SuccessResponse(result, "Customer retrieved successfully");
            }
            catch(Exception ex)
            {
                return ApiResponse<CustomerDto>
                    .InternalServerError("Failed to retrieve customer: " + ex.Message);
            }
        }
    }
}
