using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTrackerNewAPI.Application.Services;
using ExpenseTrackerNewAPI.Core.Entities;
using System.Linq;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace ExpenseTrackerNewAPI.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var category = await _categoryService.CreateCategoryAsync(userId, request.Name);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var category = await _categoryService.UpdateCategoryAsync(userId, id, request.Name);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                await _categoryService.DeleteCategoryAsync(userId, id);
                return Ok(new { Message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { Message = "Invalid token: User information not found" });

                var userId = int.Parse(userIdClaim.Value);
                var categories = await _categoryService.GetUserCategoriesAsync(userId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
    }
} 