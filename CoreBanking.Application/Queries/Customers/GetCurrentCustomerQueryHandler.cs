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
    public class GetCurrentCustomerQueryHandler : IRequestHandler<GetCurrentCustomerQuery, ApiResponse<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetCurrentCustomerQueryHandler(ICustomerRepository customerRepository, ICurrentUserService currentUserService)
        {
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }
        public async Task<ApiResponse<CustomerDto>> Handle(GetCurrentCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;

                var customer = await _customerRepository
                    .GetByUserIdAsync(userId);
                if(customer == null)
                {
                    return ApiResponse<CustomerDto>
                        .NotFound("Customer profile not found");
                }
                var result = new CustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    BVN = customer.BVN,
                    KYCStatus = customer.KYCStatus,
                    DateOfBirth = customer.DateOfBirth,
                    CreatedAt = customer.CreatedAt
                };

                return ApiResponse<CustomerDto>
                    .SuccessResponse(result, "Customer profile retrieved successfully");
            }
            catch(Exception ex)
            {
                return ApiResponse<CustomerDto>
                    .InternalServerError("Failed to retrieve profile: " + ex.Message);
            }
        }
    }
}
