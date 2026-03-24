using CoreBanking.Application.Commands.Account;
using CoreBanking.Application.Queries.Accounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
    /// <summary>
    /// Manages bank accounts: creation, retrieval, status changes, and balance inquiries.
    /// All endpoints require a valid Bearer token.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new bank account for the authenticated customer.
        /// </summary>
        /// <remarks>
        /// Opens a new account with the specified account type and currency.
        /// A unique 10-digit account number is generated automatically.
        /// The customer must have a verified profile before creating an account.
        /// </remarks>
        /// <param name="command">Account details including account type and currency.</param>
        /// <returns>The newly created account with its generated account number.</returns>
        /// <response code="200">Account created successfully.</response>
        /// <response code="400">Validation failed (e.g. invalid account type or currency).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPost("CreateAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(CreateAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all bank accounts in the system. Admin only.
        /// </summary>
        /// <remarks>
        /// Returns a list of every account across all customers.
        /// Restricted to users with the Admin role.
        /// </remarks>
        /// <returns>A list of all bank accounts.</returns>
        /// <response code="200">List of accounts returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllAccountsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Retrieves account details by account number.
        /// </summary>
        /// <remarks>
        /// Looks up a single bank account using its unique 10-digit account number.
        /// Returns full account details including status, type, currency, and balance.
        /// </remarks>
        /// <param name="accountNumber">The 10-digit account number.</param>
        /// <returns>The account details for the matching account.</returns>
        /// <response code="200">Account found and returned.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No account found with the given account number.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("GetAccountByNumber/{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByAccountNumber(string accountNumber)
        {
            var result = await _mediator.Send(new GetAccountDetailsQuery(accountNumber));
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all accounts belonging to a specific customer.
        /// </summary>
        /// <remarks>
        /// Returns every bank account linked to the given customer ID.
        /// A single customer may have multiple accounts (e.g. savings, current).
        /// </remarks>
        /// <param name="customerId">The unique identifier (GUID) of the customer.</param>
        /// <returns>A list of accounts owned by the specified customer.</returns>
        /// <response code="200">Customer accounts returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No customer found with the given ID.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("GetAccountsByCustomerId/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerAccounts(Guid customerId)
        {
            var result = await _mediator.Send(new GetCustomerAccountsQuery(customerId));
            return Ok(result);
        }

        /// <summary>
        /// Changes the status of a bank account. Admin only.
        /// </summary>
        /// <remarks>
        /// Allows an Admin to activate, freeze, or close a bank account.
        /// Account status controls whether transactions can be performed on the account.
        /// Restricted to users with the Admin role.
        /// </remarks>
        /// <param name="accountNumber">The 10-digit account number to update.</param>
        /// <param name="command">The new account status value.</param>
        /// <returns>The updated account with the new status.</returns>
        /// <response code="200">Account status updated successfully.</response>
        /// <response code="400">Validation failed (e.g. invalid status value).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="404">No account found with the given account number.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("ChangeAccountStatus/{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeAccountStatus(string accountNumber, [FromBody] ChangeAccountStatusCommand command)
        {
            command.AccountNumber = accountNumber;
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Retrieves the current balance of a bank account.
        /// </summary>
        /// <remarks>
        /// Returns the available balance for the specified account number.
        /// The authenticated user must own the account or have Admin access.
        /// </remarks>
        /// <param name="accountNumber">The 10-digit account number to query.</param>
        /// <returns>The current account balance and currency.</returns>
        /// <response code="200">Balance returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No account found with the given account number.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpGet("balance{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBalance(string accountNumber)
        {
            var result = await _mediator.Send(new GetAccountBalanceQuery(accountNumber));
            return Ok(result);
        }
    }
}
