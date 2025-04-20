using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;
using ExpenseTrackerNewAPI.Infrastructure.Data;

namespace ExpenseTrackerNewAPI.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var query = "SELECT * FROM Users";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(query);
        }

        public async Task<int> AddAsync(User entity)
        {
            var query = @"
                INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, MonthlySalary, CreatedAt, IsActive)
                VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @MonthlySalary, @CreatedAt, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, entity);
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            var query = @"
                UPDATE Users 
                SET Username = @Username,
                    Email = @Email,
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    MonthlySalary = @MonthlySalary,
                    LastLoginAt = @LastLoginAt,
                    IsActive = @IsActive,
                    RefreshToken = @RefreshToken,
                    RefreshTokenExpiryTime = @RefreshTokenExpiryTime
                WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, entity);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Users WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var query = "SELECT * FROM Users WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash, string passwordSalt)
        {
            var query = @"
                UPDATE Users 
                SET PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt
                WHERE Id = @UserId";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { UserId = userId, PasswordHash = passwordHash, PasswordSalt = passwordSalt });
            return affectedRows > 0;
        }

        public async Task<bool> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            var query = @"
                UPDATE Users 
                SET RefreshToken = @RefreshToken,
                    RefreshTokenExpiryTime = @RefreshTokenExpiryTime
                WHERE Id = @UserId";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { UserId = userId, RefreshToken = refreshToken, RefreshTokenExpiryTime = refreshTokenExpiryTime });
            return affectedRows > 0;
        }

        public async Task<bool> UpdateProfileAsync(int userId, string profilePicture, string language, string theme)
        {
            var query = @"
                UPDATE Users 
                SET ProfilePicture = @ProfilePicture,
                    Language = @Language,
                    Theme = @Theme
                WHERE Id = @UserId";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new 
            { 
                UserId = userId, 
                ProfilePicture = profilePicture, 
                Language = language, 
                Theme = theme 
            });
            return affectedRows > 0;
        }

        public async Task<bool> UpdatePreferencesAsync(int userId, int itemsPerPage, string defaultCurrency, bool notificationEnabled)
        {
            var query = @"
                IF EXISTS (SELECT 1 FROM UserPreferences WHERE UserId = @UserId)
                    UPDATE UserPreferences 
                    SET ItemsPerPage = @ItemsPerPage,
                        DefaultCurrency = @DefaultCurrency,
                        NotificationEnabled = @NotificationEnabled
                    WHERE UserId = @UserId
                ELSE
                    INSERT INTO UserPreferences (UserId, ItemsPerPage, DefaultCurrency, NotificationEnabled)
                    VALUES (@UserId, @ItemsPerPage, @DefaultCurrency, @NotificationEnabled)";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new 
            { 
                UserId = userId, 
                ItemsPerPage = itemsPerPage, 
                DefaultCurrency = defaultCurrency, 
                NotificationEnabled = notificationEnabled 
            });
            return affectedRows > 0;
        }

        public async Task<UserPreferences> GetUserPreferencesAsync(int userId)
        {
            var query = "SELECT * FROM UserPreferences WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserPreferences>(query, new { UserId = userId });
        }
    }
} 