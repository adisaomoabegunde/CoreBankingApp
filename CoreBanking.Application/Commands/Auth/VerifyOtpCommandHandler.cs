using CoreBanking.Application.Interfaces;
using CoreBanking.Domain.Common.Responses;
using CoreBanking.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Commands.Auth
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApiResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;
        private readonly IOtpRepository _otpRepository;
        private readonly IPendingRegistrationRepository _pendingRegistrationRepository;

        public VerifyOtpCommandHandler(IUserRepository userRepository, IOtpService otpService, IOtpRepository otpRepository, IPendingRegistrationRepository pendingRegistrationRepository)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _otpRepository = otpRepository;
            _pendingRegistrationRepository = pendingRegistrationRepository;
        }

        public async Task<ApiResponse<string>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pendingUser = await _pendingRegistrationRepository.GetByEmailAsync(request.Email);

                if (pendingUser == null)
                    return ApiResponse<string>.BadRequest("User not found");

                //var otp = await _otpRepository.GetValidOtp(user.Id, request.Otp);

                if(pendingUser.OtpCode != request.Otp)
                {
                    return ApiResponse<string>
                        .Unauthorized("Invalid OTP.");
                }

                if(pendingUser.ExpirationTime < DateTime.UtcNow)
                {
                    return ApiResponse<string>
                        .Unauthorized("OTP has expired.");
                }

                

                var user = new User
                {
                    Username = pendingUser.Username,
                    Email = pendingUser.Email,
                    PasswordHash = pendingUser.PasswordHash,
                    Role = pendingUser.Role,
                    IsActive = true
                };

                if (await _userRepository.EmailExistsAsync(pendingUser.Email))
                {
                    return ApiResponse<string>
                        .Unauthorized("user already exists.");
                }

                await _userRepository.AddAsync(user);

                await _pendingRegistrationRepository.DeleteAsync(pendingUser);

                return ApiResponse<string>.SuccessResponse("Account verified successfully.", "User registration complete.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError("verification failed:" + ex.Message);
            }
        }
    }
}
