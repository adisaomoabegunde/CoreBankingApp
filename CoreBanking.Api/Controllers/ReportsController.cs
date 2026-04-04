using CoreBanking.Application.Queries.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
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

        [HttpGet("daily-summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateTime? date)
        {
            var query = new GetDailySummaryQuery
            {
                Date = date
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("account-balances")]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("audit-trail")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAuditTrail([FromQuery] GetAuditTrailQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
