using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;

namespace ExpenseTrackerNewAPI.Application.Services
{
    public class ReportService
    {
        private readonly IUserRepository _userRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ReportService(
            IUserRepository userRepository,
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository)
        {
            _userRepository = userRepository;
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<MonthlyReport> GenerateMonthlyReportAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-1);

            var expenses = await _expenseRepository.GetByDateRangeAsync(userId, startDate, endDate);
            var categories = await _categoryRepository.GetByUserIdAsync(userId);

            var totalExpenses = expenses.Sum(e => e.Amount);
            var remainingBudget = user.MonthlySalary - totalExpenses;

            var categoryExpenses = new Dictionary<string, decimal>();
            foreach (var category in categories)
            {
                var categoryTotal = expenses
                    .Where(e => e.CategoryId == category.Id)
                    .Sum(e => e.Amount);
                categoryExpenses.Add(category.Name, categoryTotal);
            }

            return new MonthlyReport
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                MonthlySalary = user.MonthlySalary,
                TotalExpenses = totalExpenses,
                RemainingBudget = remainingBudget,
                CategoryExpenses = categoryExpenses,
                Expenses = expenses
            };
        }
    }

    public class MonthlyReport
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlySalary { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal RemainingBudget { get; set; }
        public Dictionary<string, decimal> CategoryExpenses { get; set; }
        public IEnumerable<Expense> Expenses { get; set; }
    }
} 