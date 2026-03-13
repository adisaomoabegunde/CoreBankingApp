using CoreBanking.Application.Commands.Customers;
using CoreBanking.Application.Queries.Customers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/customers")]
    public class CustomersController : Controller
    {
        private readonly IMediator _mediator;
        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("customer-registration")]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetCustomerById{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("UpdateCustomer{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, UpdateCustomerCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateKyc{id}")]
        public async Task<IActionResult> UpdateCustomerKycStatus(Guid id, UpdateCustomerKycStatusCommand command)
        {
            command.Id = id;

            return Ok(await _mediator.Send(command));
        }

        [HttpGet("GetCurrentCustomer")]
        public async Task<IActionResult> GetCurrentCustomer()
        {
            var result = await _mediator.Send(new  GetCurrentCustomerQuery());
            return Ok(result);
        }

    }
}
