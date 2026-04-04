using CoreBanking.Application.DTOs;
using CoreBanking.Application.Interfaces;
using CoreBanking.Application.Queries.Transactions;
using CoreBanking.Domain.Common.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Validators
{
    public class GetAuditTrailQueryHandler
    : IRequestHandler<GetAuditTrailQuery, ApiResponse<AuditLogListDto>>
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<GetAuditTrailQueryHandler> _logger;

        public GetAuditTrailQueryHandler(
            IAuditRepository auditRepository,
            ICurrentUserService currentUser,
            ILogger<GetAuditTrailQueryHandler> logger)
        {
            _auditRepository = auditRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ApiResponse<AuditLogListDto>> Handle(
            GetAuditTrailQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching audit logs");

                // 🔐 ADMIN ONLY
                if (_currentUser.Role != "Admin")
                {
                    _logger.LogWarning("Unauthorized audit log access by user {UserId}", _currentUser.UserId);

                    return ApiResponse<AuditLogListDto>
                        .Unauthorized("Only admins can access audit logs");
                }

                var (logs, totalRecords) = await _auditRepository.GetAuditLogsAsync(
                    request.UserId,
                    request.Action,
                    request.FromDate,
                    request.ToDate,
                    request.PageNumber,
                    request.PageSize
                );

                var result = logs.Select(x => new AuditLogDto
                {
                    UserId = x.UserId,
                    Action = x.Action,
                    IpAddress = x.IpAddress,
                    Description = x.Description,
                    Timestamp = x.Timestamp
                }).ToList();

                var response = new AuditLogListDto
                {
                    Logs = result,
                    TotalRecords = totalRecords
                };

                _logger.LogInformation("Retrieved {Count} audit logs", result.Count);

                return ApiResponse<AuditLogListDto>
                    .SuccessResponse(response, "Audit logs retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");

                return ApiResponse<AuditLogListDto>
                    .InternalServerError("An error occurred while retrieving audit logs");
            }
        }
    }
}
