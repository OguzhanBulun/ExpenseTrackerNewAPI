using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerNewAPI.Application.Services;
using System.Security.Claims;

namespace ExpenseTrackerNewAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await _userService.RegisterAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.MonthlySalary);

                return Ok(new { Message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (token, refreshToken) = await _userService.LoginAsync(request.Email, request.Password);
                return Ok(new { Token = token, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPasswordAsync(request.Email, request.NewPassword);
                return Ok(new { Message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update-salary")]
        public async Task<IActionResult> UpdateMonthlySalary([FromBody] UpdateSalaryRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                await _userService.UpdateMonthlySalaryAsync(userId, request.NewSalary);
                return Ok(new { Message = "Monthly salary updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var result = await _userService.UpdateProfileAsync(
                    userId,
                    request.ProfilePicture,
                    request.Language,
                    request.Theme);

                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update-preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var result = await _userService.UpdatePreferencesAsync(
                    userId,
                    request.ItemsPerPage,
                    request.DefaultCurrency,
                    request.NotificationEnabled);

                return Ok(new { Message = "Preferences updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var preferences = await _userService.GetUserPreferencesAsync(userId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal MonthlySalary { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class UpdateSalaryRequest
    {
        public decimal NewSalary { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string ProfilePicture { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
    }

    public class UpdatePreferencesRequest
    {
        public int ItemsPerPage { get; set; }
        public string DefaultCurrency { get; set; }
        public bool NotificationEnabled { get; set; }
    }
} 