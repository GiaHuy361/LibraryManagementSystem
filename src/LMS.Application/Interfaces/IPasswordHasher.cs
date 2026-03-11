namespace LMS.Application.Interfaces;

/// <summary>
/// Abstracts password hashing — Application layer depends on this interface,
/// Infrastructure provides the BCrypt implementation.
/// This follows the Dependency Inversion Principle.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string plainText, string hash);
}
