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
       

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
           
        }

        public async Task<ApiResponse<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            try
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return ApiResponse<RegisterUserResponse>
                        .FailureResponse("Email already exists.", "01");
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    IsActive = true
                };

                await _userRepository.AddAsync(user);
                var response = new RegisterUserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Role = user.Role,
                    Message = "User registered successfully"
                };

                return ApiResponse<RegisterUserResponse>
                    .SuccessResponse(response, "Registration Successfull");

            }
            catch (Exception ex)
            {
                return ApiResponse<RegisterUserResponse>
                   .FailureResponse(ex.Message, "99");

            }



            
           

        }
    }
}
