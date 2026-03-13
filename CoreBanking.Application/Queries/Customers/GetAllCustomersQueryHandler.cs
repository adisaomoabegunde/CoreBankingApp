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
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, ApiResponse<List<CustomerDto>>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<ApiResponse<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();

                var result = customers.Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    DateOfBirth = c.DateOfBirth,
                    Address = c.Address,
                    BVN = c.BVN,
                    KYCStatus = c.KYCStatus,
                    CreatedAt = c.CreatedAt
                }).ToList();

                return ApiResponse<List<CustomerDto>>
                    .SuccessResponse(result, "Customers retrieved successfully");
            }
            catch(Exception ex)
            {
                return ApiResponse<List<CustomerDto>>
                    .InternalServerError("Failed to retrieve customers: " + ex.Message);
            }
        }
    }
}
