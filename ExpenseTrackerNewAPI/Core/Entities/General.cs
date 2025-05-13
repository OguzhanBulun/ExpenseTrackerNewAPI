namespace ExpenseTrackerNewAPI.Core.Entities
{

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal MonthlySalary { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class UpdateSalaryRequest
    {
        public decimal NewSalary { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string ProfilePicture { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
    }

    public class UpdatePreferencesRequest
    {
        public int ItemsPerPage { get; set; }
        public string DefaultCurrency { get; set; }
        public bool NotificationEnabled { get; set; }
    }
}
