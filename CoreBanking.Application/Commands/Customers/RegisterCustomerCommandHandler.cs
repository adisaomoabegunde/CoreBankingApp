using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IEventProducer _eventProducer;
        private readonly ILogger<RegisterCustomerCommandHandler> _logger;

        public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, ICurrentUserService currentUserService, IEventProducer eventProducer, ILogger<RegisterCustomerCommandHandler> logger )
        {
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _eventProducer = eventProducer;
            _logger = logger;
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
                    KYCStatus = Domain.Enums.KycStatus.Pending
                };
                await _customerRepository.AddAsync(customer);

                try
                {
                    var customerEvent = new CustomerRegisteredEvent
                    {
                        CustomerId = customer.Id,
                        Email = customer.Email,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        PhoneNumber = customer.PhoneNumber,
                        Reference = $"CUST-{DateTime.UtcNow.Ticks}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _eventProducer.PublishAsync("customer.registered", customerEvent);

                    _logger.LogInformation("Customer registered event published: {Reference}", customerEvent.Reference);
                }
                catch(Exception KafkaEx)
                {
                    _logger.LogError(KafkaEx, "Failed to publish customer registered event");
                }

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
