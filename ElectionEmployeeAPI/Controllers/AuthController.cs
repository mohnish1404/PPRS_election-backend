using ElectionEmployeeAPI.DTOs;
using ElectionEmployeeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ElectionEmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly OtpService _otpService;

        public AuthController(AuthService authService, OtpService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        // ✅ Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message) = await _authService.RegisterAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        // ✅ Login Without OTP
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message, token, role) =
                await _authService.LoginWithoutOtpAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message, token, role });
        }

        // ✅ Send OTP
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message, otpCode) =
                await _otpService.GenerateOtpAsync(dto.UserId, dto.MobileNumber);

            if (!success)
                return BadRequest(new { success = false, message });

            // Development mein OTP response mein bhej rahe hain
            // Production mein SMS se bhejenge
            return Ok(new { success = true, message, otpCode });
        }

        // ✅ Verify OTP & Login
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (otpSuccess, otpMessage) =
                await _otpService.VerifyOtpAsync(dto.UserId, dto.OtpCode!);

            if (!otpSuccess)
                return BadRequest(new { success = false, message = otpMessage });

            var loginDto = new LoginDto
            {
                UserId = dto.UserId,
                Password = "",
                LoginWithOtp = true
            };

            var user = await _authService.GetUserByIdAsync(dto.UserId);
            if (user == null)
                return BadRequest(new { success = false, message = "User not found" });

            // Use Value to access the tuple fields when the method returns a nullable tuple
            var token = user.Value.Token;
            var role = user.Value.Role;

            return Ok(new { success = true, message = "Login successful", token, role });
        }

        // ✅ Send Reset OTP
        [HttpPost("send-reset-otp")]
        public async Task<IActionResult> SendResetOtp([FromBody] SendResetOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message, otpCode) =
                await _authService.SendResetOtpAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            // Development mein OTP response mein bhej rahe hain
            return Ok(new { success = true, message, otpCode });
        }

        // ✅ Verify Reset OTP
        [HttpPost("verify-reset-otp")]
        public async Task<IActionResult> VerifyResetOtp([FromBody] VerifyResetOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message) =
                await _authService.VerifyResetOtpAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        // ✅ Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message) =
                await _authService.ResetPasswordAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        // ✅ Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message) = await _authService.ForgotPasswordAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        // ✅ Activate User
        [HttpPost("activate-user")]
        public async Task<IActionResult> ActivateUser([FromBody] ActivateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data entered." });

            var (success, message) = await _authService.ActivateUserAsync(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }

        [HttpGet("my-approval-status")]
        [Authorize]
        public async Task<IActionResult> GetMyApprovalStatus()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _authService.GetMyApprovalStatusAsync(userId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("my-profile")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _authService.GetUserProfileAsync(userId);
            if (user == null) return NotFound(new { success = false, message = "User not found" });

            return Ok(new { success = true, data = user });
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var (success, message) = await _authService.UpdateUserProfileAsync(userId, dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
    }
}