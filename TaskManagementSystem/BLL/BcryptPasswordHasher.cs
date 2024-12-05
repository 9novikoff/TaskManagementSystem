namespace TaskManagementSystem.BLL;

/// <summary>
/// Provides methods for securely hashing and verifying passwords using the BCrypt algorithm.
/// Read more about BCrypt: https://en.wikipedia.org/wiki/Bcrypt
/// </summary>
public static class BcryptPasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}