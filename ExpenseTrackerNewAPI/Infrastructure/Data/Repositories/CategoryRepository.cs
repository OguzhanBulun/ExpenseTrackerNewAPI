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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DapperContext _context;

        public CategoryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Categories WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Category>(query, new { Id = id });
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var query = "SELECT * FROM Categories";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Category>(query);
        }

        public async Task<int> AddAsync(Category entity)
        {
            var query = @"
                INSERT INTO Categories (Name, UserId, CreatedAt)
                VALUES (@Name, @UserId, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, entity);
        }

        public async Task<bool> UpdateAsync(Category entity)
        {
            var query = @"
                UPDATE Categories 
                SET Name = @Name,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, entity);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Categories WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
        {
            var query = "SELECT * FROM Categories WHERE UserId = @UserId ORDER BY Name";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Category>(query, new { UserId = userId });
        }

        public async Task<bool> IsCategoryNameExistsAsync(int userId, string categoryName)
        {
            var query = "SELECT COUNT(1) FROM Categories WHERE UserId = @UserId AND Name = @CategoryName";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId, CategoryName = categoryName });
            return count > 0;
        }
    }
} 