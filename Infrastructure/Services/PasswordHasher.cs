using Application.Common.Interfaces;

namespace Infrastructure.Services;

/// <summary>
/// Реализация IPasswordHasher через BCrypt.Net-Next.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
