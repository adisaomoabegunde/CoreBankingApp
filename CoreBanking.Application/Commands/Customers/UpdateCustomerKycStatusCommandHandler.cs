using CoreBanking.Application.Events;
using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IEventProducer _eventProducer;
        private readonly ILogger<UpdateCustomerKycStatusCommandHandler> _logger;

        public UpdateCustomerKycStatusCommandHandler(ICustomerRepository customerRepository, IEventProducer eventProducer, ILogger<UpdateCustomerKycStatusCommandHandler> logger)
        {
            _customerRepository = customerRepository;
            _eventProducer = eventProducer;
            _logger = logger;
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

                try
                {
                    var kycEvent = new CustomerKycUpdatedEvent
                    {
                        CustomerId = customer.Id,
                        KycStatus = customer.KYCStatus,
                        Reference = $"KYC-{DateTime.UtcNow.Ticks}",
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _eventProducer.PublishAsync("customer.kyc.updated", kycEvent);
                    _logger.LogInformation("Customer KYC updated event published: {Reference}", kycEvent.Reference);

                    
                }catch(Exception KafkaEx)
                {
                    _logger.LogError(KafkaEx, "Failed to publish customer KYC updated event");
                }

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
