namespace GymManagement.Services
{
    public static class PasswordHasher
    {
        // used to hash the password before storing it in the database
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // used to verify the password against the hashed password stored in the database
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}