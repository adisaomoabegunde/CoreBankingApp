using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Customers
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, ApiResponse<string>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;

        public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, ICurrentUserService currentUserService)
        {
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }
        public async Task<ApiResponse<string>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _customerRepository.EmailExistsAsync(request.Email))
                    return ApiResponse<string>.Duplicate("Customer email already exists");
                if (await _customerRepository.PhoneExistsAsync(request.PhoneNumber))
                    return ApiResponse<string>.Duplicate("Phone number already exists");
                if (await _customerRepository.BvnExistsAsync(request.BVN))
                    return ApiResponse<string>.Duplicate("BVN already exists");

                var userId = _currentUserService.UserId;
                var customer = new Customer
                {
                    UserId = userId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Address = request.Address,
                    BVN = request.BVN,
                    KYCStatus = "Pending"
                };
                await _customerRepository.AddAsync(customer);

                return ApiResponse<string>
                    .SuccessResponse("Customer registered successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>
                    .InternalServerError("Customer registration failed: " + ex.Message);
            }
        }
    }
}
