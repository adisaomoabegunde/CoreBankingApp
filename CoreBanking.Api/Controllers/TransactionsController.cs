using CoreBanking.Application.Commands.Transactions;
using CoreBanking.Application.Queries.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
    /// <summary>
    /// Handles financial transactions: transfers, deposits, withdrawals, transaction history, and reversals.
    /// All endpoints require a valid Bearer token.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Transfers funds between two accounts.
        /// </summary>
        /// <remarks>
        /// Moves the specified amount from the source account to the destination account.
        /// Both accounts must be active. The source account must have sufficient balance.
        /// An idempotency key should be provided to prevent duplicate transfers.
        /// </remarks>
        /// <param name="command">Transfer details including source account, destination account, amount, and description.</param>
        /// <returns>The completed transaction record with reference number.</returns>
        /// <response code="200">Transfer completed successfully.</response>
        /// <response code="400">Validation failed (e.g. insufficient balance, same source and destination, inactive account).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Source or destination account not found.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Transfer([FromBody] TransferCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Deposits funds into an account. Admin only.
        /// </summary>
        /// <remarks>
        /// Credits the specified amount to the target account.
        /// This is an administrative operation typically used for cash deposits or adjustments.
        /// Restricted to users with the Admin role.
        /// </remarks>
        /// <param name="command">Deposit details including account number, amount, and description.</param>
        /// <returns>The completed deposit transaction record.</returns>
        /// <response code="200">Deposit completed successfully.</response>
        /// <response code="400">Validation failed (e.g. invalid amount, inactive account).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="404">Account not found.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("deposit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Withdraws funds from an account.
        /// </summary>
        /// <remarks>
        /// Debits the specified amount from the target account.
        /// The account must be active and have sufficient balance to cover the withdrawal.
        /// </remarks>
        /// <param name="command">Withdrawal details including account number, amount, and description.</param>
        /// <returns>The completed withdrawal transaction record.</returns>
        /// <response code="200">Withdrawal completed successfully.</response>
        /// <response code="400">Validation failed (e.g. insufficient balance, inactive account).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Account not found.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpPost("withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves paginated transaction history for an account.
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of all transactions (deposits, withdrawals, transfers) for the specified account.
        /// Results can be filtered by date range using the optional fromDate and toDate parameters.
        /// </remarks>
        /// <param name="accountNumber">The 10-digit account number to query.</param>
        /// <param name="fromDate">Optional start date filter (inclusive).</param>
        /// <param name="toDate">Optional end date filter (inclusive).</param>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of records per page (default: 10).</param>
        /// <returns>A paginated list of transactions for the account.</returns>
        /// <response code="200">Transaction history returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Account not found.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpGet("viewTransaction/{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccountTransactions(string accountNumber, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAccountTransactionQuery(accountNumber)
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single transaction by its reference number.
        /// </summary>
        /// <remarks>
        /// Looks up a specific transaction using its unique reference string.
        /// Returns the full transaction details including amount, type, status, and involved accounts.
        /// </remarks>
        /// <param name="reference">The unique transaction reference string.</param>
        /// <returns>The matching transaction record.</returns>
        /// <response code="200">Transaction found and returned.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No transaction found with the given reference.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpGet("viewTransactionByReference{reference}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransaction(string reference)
        {
            var result = await _mediator.Send(new GetTransactionByReferenceQuery(reference));
            return Ok(result);
        }

        /// <summary>
        /// Reverses a completed transaction. Admin only.
        /// </summary>
        /// <remarks>
        /// Creates a reversal transaction that undoes the original transaction.
        /// The original transaction's funds are returned to the source account.
        /// A transaction can only be reversed once. Restricted to users with the Admin role.
        /// </remarks>
        /// <param name="reference">The reference number of the transaction to reverse.</param>
        /// <returns>The reversal transaction record.</returns>
        /// <response code="200">Transaction reversed successfully.</response>
        /// <response code="400">Transaction has already been reversed or cannot be reversed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="404">No transaction found with the given reference.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("{reference}/reverse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReverseTransaction(string reference)
        {
            var result = await _mediator.Send(new ReverseTransactionCommand(reference));
            return Ok(result);
        }
    }
}
