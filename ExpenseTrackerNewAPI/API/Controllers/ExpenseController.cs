using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerNewAPI.Application.Services;
using ExpenseTrackerNewAPI.Core.Entities;
using System.Security.Claims;

namespace ExpenseTrackerNewAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseService _expenseService;

        public ExpenseController(ExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Geçersiz token: Kullanıcı bilgisi bulunamadı" });

                var userId = int.Parse(userIdClaim.Value);
                var expense = await _expenseService.CreateExpenseAsync(
                    userId,
                    request.Name,
                    request.Amount,
                    request.Date,
                    request.CategoryId,
                    request.Description);

                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Geçersiz token: Kullanıcı bilgisi bulunamadı" });

                var userId = int.Parse(userIdClaim.Value);
                var expense = await _expenseService.UpdateExpenseAsync(
                    userId,
                    id,
                    request.Name,
                    request.Amount,
                    request.Date,
                    request.CategoryId,
                    request.Description);

                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                await _expenseService.DeleteExpenseAsync(userId, id);
                return Ok(new { Message = "Expense deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Geçersiz token: Kullanıcı bilgisi bulunamadı" });

                var userId = int.Parse(userIdClaim.Value);
                IEnumerable<Expense> expenses;

                if (startDate.HasValue && endDate.HasValue)
                {
                    expenses = await _expenseService.GetExpensesByDateRangeAsync(userId, startDate.Value, endDate.Value);
                }
                else
                {
                    expenses = await _expenseService.GetUserExpensesAsync(userId);
                }

                return Ok(expenses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetExpensesByCategory(int categoryId)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Geçersiz token: Kullanıcı bilgisi bulunamadı" });

                var userId = int.Parse(userIdClaim.Value);
                var expenses = await _expenseService.GetExpensesByCategoryAsync(userId, categoryId);
                return Ok(expenses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalExpenses([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Geçersiz token: Kullanıcı bilgisi bulunamadı" });

                var userId = int.Parse(userIdClaim.Value);
                var total = await _expenseService.GetTotalExpensesAsync(userId, startDate, endDate);
                return Ok(new { Total = total });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedExpenses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? categoryId = null)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var (expenses, totalCount) = await _expenseService.GetPaginatedExpensesAsync(
                    userId,
                    pageNumber,
                    pageSize,
                    startDate,
                    endDate,
                    categoryId);

                return Ok(new
                {
                    Expenses = expenses,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class CreateExpenseRequest
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
    }
} 