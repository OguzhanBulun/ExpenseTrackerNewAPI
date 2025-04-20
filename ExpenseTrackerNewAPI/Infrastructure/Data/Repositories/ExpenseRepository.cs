using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;
using ExpenseTrackerNewAPI.Infrastructure.Data;

namespace ExpenseTrackerNewAPI.Infrastructure.Data.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DapperContext _context;

        public ExpenseRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Expenses WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Expense>(query, new { Id = id });
        }

        public async Task<IEnumerable<Expense>> GetAllAsync()
        {
            var query = "SELECT * FROM Expenses";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Expense>(query);
        }

        public async Task<int> AddAsync(Expense entity)
        {
            var query = @"
                INSERT INTO Expenses (Name, Amount, Date, CategoryId, UserId, Description, CreatedAt)
                VALUES (@Name, @Amount, @Date, @CategoryId, @UserId, @Description, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, entity);
        }

        public async Task<bool> UpdateAsync(Expense entity)
        {
            var query = @"
                UPDATE Expenses 
                SET Name = @Name,
                    Amount = @Amount,
                    Date = @Date,
                    CategoryId = @CategoryId,
                    Description = @Description,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, entity);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Expenses WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<Expense>> GetByUserIdAsync(int userId)
        {
            var query = "SELECT * FROM Expenses WHERE UserId = @UserId ORDER BY Date DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Expense>(query, new { UserId = userId });
        }

        public async Task<IEnumerable<Expense>> GetByCategoryIdAsync(int categoryId)
        {
            var query = "SELECT * FROM Expenses WHERE CategoryId = @CategoryId ORDER BY Date DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Expense>(query, new { CategoryId = categoryId });
        }

        public async Task<IEnumerable<Expense>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var query = @"
                SELECT * FROM Expenses 
                WHERE UserId = @UserId 
                AND Date BETWEEN @StartDate AND @EndDate
                ORDER BY Date DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Expense>(query, new { UserId = userId, StartDate = startDate, EndDate = endDate });
        }

        public async Task<decimal> GetTotalExpensesByUserIdAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = "SELECT SUM(Amount) FROM Expenses WHERE UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            if (startDate.HasValue && endDate.HasValue)
            {
                query += " AND Date BETWEEN @StartDate AND @EndDate";
                parameters.Add("StartDate", startDate.Value);
                parameters.Add("EndDate", endDate.Value);
            }

            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteScalarAsync<decimal?>(query, parameters);
            return result ?? 0;
        }

        public async Task<(IEnumerable<Expense> Expenses, int TotalCount)> GetPaginatedExpensesAsync(
            int userId,
            int pageNumber,
            int pageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? categoryId = null)
        {
            var offset = (pageNumber - 1) * pageSize;
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var whereClause = "WHERE UserId = @UserId";
            if (startDate.HasValue && endDate.HasValue)
            {
                whereClause += " AND Date BETWEEN @StartDate AND @EndDate";
                parameters.Add("StartDate", startDate.Value);
                parameters.Add("EndDate", endDate.Value);
            }
            if (categoryId.HasValue)
            {
                whereClause += " AND CategoryId = @CategoryId";
                parameters.Add("CategoryId", categoryId.Value);
            }

            var query = $@"
                SELECT * FROM Expenses 
                {whereClause}
                ORDER BY Date DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*) FROM Expenses 
                {whereClause}";

            using var connection = _context.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(query, parameters);
            
            var expenses = await multi.ReadAsync<Expense>();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (expenses, totalCount);
        }
    }
} 