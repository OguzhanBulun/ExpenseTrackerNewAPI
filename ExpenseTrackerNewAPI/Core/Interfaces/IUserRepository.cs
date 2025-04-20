using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;

namespace ExpenseTrackerNewAPI.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash, string passwordSalt);
        Task<bool> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime refreshTokenExpiryTime);
        Task<bool> UpdateProfileAsync(int userId, string profilePicture, string language, string theme);
        Task<bool> UpdatePreferencesAsync(int userId, int itemsPerPage, string defaultCurrency, bool notificationEnabled);
        Task<UserPreferences> GetUserPreferencesAsync(int userId);
    }
} 