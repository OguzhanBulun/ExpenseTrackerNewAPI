using System;
using System.Collections.Generic;

namespace ExpenseTrackerNewAPI.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public decimal MonthlySalary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string ProfilePicture { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        public UserPreferences Preferences { get; set; }
    }

    public class UserPreferences
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemsPerPage { get; set; }
        public string DefaultCurrency { get; set; }
        public bool NotificationEnabled { get; set; }
    }
} 