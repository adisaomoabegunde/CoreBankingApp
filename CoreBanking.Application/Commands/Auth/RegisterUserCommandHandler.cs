using CoreBanking.Domain.Entities;
using CoreBanking.Application.Interfaces;
using MediatR;
using CoreBanking.Domain.Common.Exceptions;
using System;
using CoreBanking.Domain.Common.Responses;


namespace CoreBanking.Application.Commands.Auth
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<RegisterUserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IOtpService _otpService;
        private readonly IPendingRegistrationRepository _pendingRegistrationRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository, IOtpService otpService, IOtpRepository otpRepository, IPendingRegistrationRepository pendingRegistrationRepository)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _otpRepository = otpRepository;
            _pendingRegistrationRepository = pendingRegistrationRepository;
        }

        public async Task<ApiResponse<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            try
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return ApiResponse<RegisterUserResponse>
                        .Unauthorized("Email already exists.");
                }

                var otpCode = _otpService.GenerateOtp();


                var pendingUser = new PendingRegistration
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    OtpCode = otpCode,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(5)
                };

                await _pendingRegistrationRepository.AddAsync(pendingUser);


                //var otp = new Otp
                //{
                //    UserId = user.Id,
                //    Code = otpCode,
                //    ExpirationTime = DateTime.UtcNow.AddMinutes(5)
                //};

                //await _otpRepository.AddAsync(otp);

                var response = new RegisterUserResponse
                {
                    Id = pendingUser.Id,
                    Email = pendingUser.Email,
                    Username = pendingUser.Username,
                    Role = pendingUser.Role,
                    Otp = otpCode
                };

                return ApiResponse<RegisterUserResponse>
                    .SuccessResponse(response, "OTP has been sent to your email. Kindly confirm OTP.");

            }
            catch (Exception ex)
            {
                return ApiResponse<RegisterUserResponse>
                   .InternalServerError("Registration failed: " + ex.Message);

            }



            
           

        }
    }
}
