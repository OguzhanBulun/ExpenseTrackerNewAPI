using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;

namespace ExpenseTrackerNewAPI.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateCategoryAsync(int userId, string name)
        {
            if (await _categoryRepository.IsCategoryNameExistsAsync(userId, name))
                throw new Exception("Category name already exists");

            var category = new Category
            {
                Name = name,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            category.Id = await _categoryRepository.AddAsync(category);
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int userId, int categoryId, string name)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                throw new Exception("Category not found or not authorized");

            if (await _categoryRepository.IsCategoryNameExistsAsync(userId, name))
                throw new Exception("Category name already exists");

            category.Name = name;
            category.UpdatedAt = DateTime.Now;

            await _categoryRepository.UpdateAsync(category);
            return category;
        }

        public async Task DeleteCategoryAsync(int userId, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                throw new Exception("Category not found or not authorized");

            await _categoryRepository.DeleteAsync(categoryId);
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(int userId)
        {
            return await _categoryRepository.GetByUserIdAsync(userId);
        }
    }
} 