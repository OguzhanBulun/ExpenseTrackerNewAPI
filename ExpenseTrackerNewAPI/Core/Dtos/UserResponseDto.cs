using ExpenseTrackerNewAPI.Core.Entities;

namespace ExpenseTrackerNewAPI.Core.Dtos
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public decimal MonthlySalary { get; set; }
        public string ProfilePicture { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        public UserPreferences Preferences { get; set; }
    }
}
