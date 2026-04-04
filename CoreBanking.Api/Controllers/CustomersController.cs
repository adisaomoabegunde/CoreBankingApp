using CoreBanking.Application.Commands.Customers;
using CoreBanking.Application.Queries.Customers;
using CoreBanking.Application.Queries.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
    /// <summary>
    /// Manages customer profiles: registration, retrieval, updates, and KYC status management.
    /// All endpoints require a valid Bearer token.
    /// </summary>
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

        /// <summary>
        /// Registers a new customer profile for the authenticated user.
        /// </summary>
        /// <remarks>
        /// Creates a customer record linked to the currently logged-in user account.
        /// The user must be authenticated. A user can only register one customer profile.
        /// </remarks>
        /// <param name="command">Customer details including full name, phone, address, and date of birth.</param>
        /// <returns>The newly created customer profile.</returns>
        /// <response code="200">Customer profile created successfully.</response>
        /// <response code="400">Validation failed (e.g. missing required fields).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPost("customer-registration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all customer profiles. Admin only.
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of all customers in the system.
        /// Restricted to users with the Admin role.
        /// </remarks>
        /// <returns>A list of all customer profiles.</returns>
        /// <response code="200">List of customers returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllCustomers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific customer by their unique ID. Admin only.
        /// </summary>
        /// <remarks>
        /// Looks up a single customer record by the provided GUID.
        /// Restricted to users with the Admin role.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the customer.</param>
        /// <returns>The matching customer profile.</returns>
        /// <response code="200">Customer found and returned.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="404">No customer found with the given ID.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetCustomerById{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Updates the profile details of a specific customer.
        /// </summary>
        /// <remarks>
        /// Allows the authenticated user to update their own customer profile fields
        /// such as phone number, address, or date of birth.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the customer to update.</param>
        /// <param name="command">The updated customer fields.</param>
        /// <returns>The updated customer profile.</returns>
        /// <response code="200">Customer profile updated successfully.</response>
        /// <response code="400">Validation failed (e.g. invalid field values).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No customer found with the given ID.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPut("UpdateCustomer{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer(Guid id, UpdateCustomerCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Updates the KYC (Know Your Customer) verification status of a customer. Admin only.
        /// </summary>
        /// <remarks>
        /// Allows an Admin to approve or reject a customer's KYC verification.
        /// KYC status controls whether a customer is eligible for financial transactions.
        /// Restricted to users with the Admin role.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the customer.</param>
        /// <param name="command">The new KYC status value.</param>
        /// <returns>The updated customer record with the new KYC status.</returns>
        /// <response code="200">KYC status updated successfully.</response>
        /// <response code="400">Validation failed (e.g. invalid status value).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="404">No customer found with the given ID.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateKyc{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerKycStatus(Guid id, UpdateCustomerKycStatusCommand command)
        {
            command.Id = id;
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Retrieves the customer profile of the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// Returns the customer record associated with the JWT token of the logged-in user.
        /// Useful for profile pages where the customer views their own information.
        /// </remarks>
        /// <returns>The customer profile linked to the current user session.</returns>
        /// <response code="200">Current customer profile returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No customer profile found for the current user.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("GetCurrentCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentCustomer()
        {
            var result = await _mediator.Send(new GetCurrentCustomerQuery());
            return Ok(result);
        }

        /// <summary>
        /// Generates an account statement for the specified account.
        /// </summary>
        /// <remarks>
        /// Returns a paginated statement showing all transactions (credits and debits) for the given account.
        /// Results can be filtered by date range. Includes opening balance, closing balance, and individual transaction entries.
        /// </remarks>
        /// <param name="accountNumber">The 10-digit account number.</param>
        /// <param name="fromDate">Optional start date filter (inclusive).</param>
        /// <param name="toDate">Optional end date filter (inclusive).</param>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of records per page (default: 10).</param>
        /// <returns>A paginated account statement with transaction entries.</returns>
        /// <response code="200">Account statement generated successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Account not found.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpGet("{accountNumber}/statement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatement(string accountNumber, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAccountStatementQuery
            {
                AccountNumber = accountNumber,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
