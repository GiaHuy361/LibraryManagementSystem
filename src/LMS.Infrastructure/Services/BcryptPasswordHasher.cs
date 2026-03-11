using LMS.Application.Interfaces;

namespace LMS.Infrastructure.Services;

/// <summary>BCrypt implementation of IPasswordHasher. Lives in Infrastructure — keeps Application clean.</summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string plainText, string hash) => BCrypt.Net.BCrypt.Verify(plainText, hash);
}
