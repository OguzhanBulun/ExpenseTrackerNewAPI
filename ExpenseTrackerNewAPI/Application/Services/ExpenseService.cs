using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;

namespace ExpenseTrackerNewAPI.Application.Services
{
    public class ExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Expense> CreateExpenseAsync(int userId, string name, decimal amount, DateTime date, int categoryId, string description)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                throw new Exception("Category not found or not authorized");

            var expense = new Expense
            {
                Name = name,
                Amount = amount,
                Date = date,
                CategoryId = categoryId,
                UserId = userId,
                Description = description,
                CreatedAt = DateTime.Now
            };

            expense.Id = await _expenseRepository.AddAsync(expense);
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(int userId, int expenseId, string name, decimal amount, DateTime date, int categoryId, string description)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId)
                throw new Exception("Expense not found or not authorized");

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                throw new Exception("Category not found or not authorized");

            expense.Name = name;
            expense.Amount = amount;
            expense.Date = date;
            expense.CategoryId = categoryId;
            expense.Description = description;
            expense.UpdatedAt = DateTime.Now;

            await _expenseRepository.UpdateAsync(expense);
            return expense;
        }

        public async Task DeleteExpenseAsync(int userId, int expenseId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId)
                throw new Exception("Expense not found or not authorized");

            await _expenseRepository.DeleteAsync(expenseId);
        }

        public async Task<IEnumerable<Expense>> GetUserExpensesAsync(int userId)
        {
            return await _expenseRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.UserId != userId)
                throw new Exception("Category not found or not authorized");

            return await _expenseRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _expenseRepository.GetByDateRangeAsync(userId, startDate, endDate);
        }

        public async Task<decimal> GetTotalExpensesAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _expenseRepository.GetTotalExpensesByUserIdAsync(userId, startDate, endDate);
        }

        public async Task<(IEnumerable<Expense> Expenses, int TotalCount)> GetPaginatedExpensesAsync(
            int userId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? categoryId = null)
        {
            if (categoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId.Value);
                if (category == null || category.UserId != userId)
                    throw new Exception("Category not found or not authorized");
            }

            return await _expenseRepository.GetPaginatedExpensesAsync(
                userId,
                pageNumber,
                pageSize,
                startDate,
                endDate,
                categoryId);
        }
    }
} 