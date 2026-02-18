using CoreBanking.Domain.Entities;
using CoreBanking.Application.Interfaces;
using MediatR;
using CoreBanking.Domain.Common.Exceptions;
using System;


namespace CoreBanking.Application.Commands.Auth
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _userRepository;
       

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
           
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
           if(await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new BadRequestException("Email already exists");
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
            return new RegisterUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                Message = "User registered successfully"
            };

        }
    }
}
