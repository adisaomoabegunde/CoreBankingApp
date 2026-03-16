using CoreBanking.Application.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.Api.Controllers
{
    /// <summary>
    /// Handles user authentication: registration, OTP verification, login, and logout.
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <remarks>
        /// Creates a pending registration and sends a one-time password (OTP) to the provided email.
        /// The account is not active until the OTP is verified via the verify-otp endpoint.
        /// </remarks>
        /// <param name="command">The registration details: username, email, password, and role.</param>
        /// <returns>A response containing the OTP code and the pending registration details.</returns>
        /// <response code="200">Registration initiated — OTP sent to email.</response>
        /// <response code="400">Validation failed (e.g. missing fields, invalid email format).</response>
        /// <response code="401">Email already exists.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Verifies the OTP sent during registration.
        /// </summary>
        /// <remarks>
        /// Validates the OTP against the pending registration. On success, the user account is
        /// activated and becomes eligible to log in.
        /// </remarks>
        /// <param name="command">The email and OTP code to verify.</param>
        /// <returns>Confirmation that the account has been activated.</returns>
        /// <response code="200">OTP verified — account activated successfully.</response>
        /// <response code="400">Invalid or expired OTP.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPost("verify-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyOtp(VerifyOtpCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token.
        /// </summary>
        /// <remarks>
        /// Validates the credentials against active accounts only. Returns a signed JWT token
        /// to be included as a Bearer token in subsequent requests.
        /// </remarks>
        /// <param name="command">The user's email and password.</param>
        /// <returns>A JWT token and user information on successful authentication.</returns>
        /// <response code="200">Login successful — JWT token returned.</response>
        /// <response code="400">Validation failed (e.g. missing email or password).</response>
        /// <response code="401">Invalid credentials or account not active.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// Invalidates the current JWT token by adding it to the token blacklist.
        /// Subsequent requests using this token will be rejected.
        /// Requires a valid Bearer token in the Authorization header.
        /// </remarks>
        /// <returns>Confirmation that the session has been terminated.</returns>
        /// <response code="200">Logout successful — token invalidated.</response>
        /// <response code="401">No valid Bearer token provided.</response>
        /// <response code="500">An unexpected server error occurred.</response>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return Ok(result);
        }
    }
}
