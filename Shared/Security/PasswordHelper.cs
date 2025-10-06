using Microsoft.AspNetCore.Identity;
namespace TaskPilot.Server
{
    public class PasswordHelper
    {
        private static readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

        // Hash the password before saving
        public static string HashPassword(string plainPassword)
        {
            // The object parameter is unused here, but required by the API
            return _hasher.HashPassword(null, plainPassword);
        }

        // Verify a password against the stored hash
        public static bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }

    }
}
