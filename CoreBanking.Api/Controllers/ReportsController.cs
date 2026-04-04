using CoreBanking.Application.Queries.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
    /// <summary>
    /// Provides administrative reports: daily summaries, account balances, and audit trails.
    /// All endpoints are restricted to Admin users.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/v1/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Generates a daily transaction summary report. Admin only.
        /// </summary>
        /// <remarks>
        /// Returns an aggregated summary of all transactions for the specified date,
        /// including total deposits, withdrawals, transfers, and net movement.
        /// Defaults to today's date if no date is provided.
        /// </remarks>
        /// <param name="date">Optional date to generate the summary for (defaults to today).</param>
        /// <returns>A summary of the day's transaction activity.</returns>
        /// <response code="200">Daily summary generated successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("daily-summary")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateTime? date)
        {
            var query = new GetDailySummaryQuery
            {
                Date = date
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of all account balances. Admin only.
        /// </summary>
        /// <remarks>
        /// Returns every account in the system with its current balance, account type, status, and currency.
        /// Useful for reconciliation and oversight purposes.
        /// </remarks>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of records per page (default: 10).</param>
        /// <returns>A paginated list of accounts with their balances.</returns>
        /// <response code="200">Account balances returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("account-balances")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAccountBalances([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
                    var query = new GetAllAccountBalancesQuery
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };

                    var result = await _mediator.Send(query);
                    return Ok(result);
        }

        /// <summary>
        /// Retrieves the audit trail log. Admin only.
        /// </summary>
        /// <remarks>
        /// Returns a filtered and paginated log of all auditable actions performed in the system,
        /// including user registrations, logins, transactions, and account status changes.
        /// Supports filtering by user, action type, and date range.
        /// </remarks>
        /// <param name="query">Filter parameters including user ID, action type, date range, and pagination.</param>
        /// <returns>A paginated list of audit log entries.</returns>
        /// <response code="200">Audit trail returned successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Admin role.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpGet("audit-trail")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuditTrail([FromQuery] GetAuditTrailQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
