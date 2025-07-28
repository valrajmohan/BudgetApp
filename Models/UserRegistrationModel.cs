using System.ComponentModel.DataAnnotations;

namespace MyBudgetTracker.Models
{
    public class UserRegistrationModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        // Ensure there's no Reset property unless it's necessary
        public string? ResetCode { get; set; }
    }

    public class LoginModel
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
    }

}
