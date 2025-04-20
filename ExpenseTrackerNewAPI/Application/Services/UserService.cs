using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;

namespace ExpenseTrackerNewAPI.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthenticationService _authenticationService;

        public UserService(IUserRepository userRepository, AuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
        }

        public async Task<User> RegisterAsync(string username, string email, string password, decimal monthlySalary)
        {
            if (await _userRepository.GetByEmailAsync(email) != null)
                throw new Exception("Email already exists");

            if (await _userRepository.GetByUsernameAsync(username) != null)
                throw new Exception("Username already exists");

            var (passwordHash, passwordSalt) = CreatePasswordHash(password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                MonthlySalary = monthlySalary,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            user.Id = await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<(string Token, string RefreshToken)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid email or password");

            if (!user.IsActive)
                throw new Exception("User is not active");

            var token = await _authenticationService.GenerateJwtToken(user);
            var refreshToken = await _authenticationService.GenerateRefreshToken();
            await _authenticationService.UpdateUserRefreshToken(user.Id, refreshToken);

            return (token, refreshToken);
        }

        public async Task ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found");

            var (passwordHash, passwordSalt) = CreatePasswordHash(newPassword);
            await _userRepository.UpdatePasswordAsync(user.Id, passwordHash, passwordSalt);
        }

        public async Task UpdateMonthlySalaryAsync(int userId, decimal newSalary)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.MonthlySalary = newSalary;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> UpdateProfileAsync(int userId, string profilePicture, string language, string theme)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return await _userRepository.UpdateProfileAsync(userId, profilePicture, language, theme);
        }

        public async Task<bool> UpdatePreferencesAsync(int userId, int itemsPerPage, string defaultCurrency, bool notificationEnabled)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return await _userRepository.UpdatePreferencesAsync(userId, itemsPerPage, defaultCurrency, notificationEnabled);
        }

        public async Task<UserPreferences> GetUserPreferencesAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return await _userRepository.GetUserPreferencesAsync(userId);
        }

        private (string Hash, string Salt) CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return (hash, salt);
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }
    }
} 