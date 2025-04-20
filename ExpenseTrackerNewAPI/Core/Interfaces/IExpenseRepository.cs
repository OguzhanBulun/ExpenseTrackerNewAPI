using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;

namespace ExpenseTrackerNewAPI.Core.Interfaces
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Expense>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalExpensesByUserIdAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<(IEnumerable<Expense> Expenses, int TotalCount)> GetPaginatedExpensesAsync(
            int userId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? categoryId = null);
    }
} 