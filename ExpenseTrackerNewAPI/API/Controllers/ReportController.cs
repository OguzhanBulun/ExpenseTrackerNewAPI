using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerNewAPI.Application.Services;
using System.Security.Claims;

namespace ExpenseTrackerNewAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyReport()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Geçersiz token: Kullanıcı bilgisi bulunamadı" });

                var userId = int.Parse(userIdClaim.Value);
                var report = await _reportService.GenerateMonthlyReportAsync(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
} 