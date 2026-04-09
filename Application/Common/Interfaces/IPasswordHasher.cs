namespace Application.Common.Interfaces;

/// <summary>
/// Абстракция для хеширования и проверки паролей.
/// Реализация находится в Infrastructure (BCrypt).
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Создаёт хеш пароля.</summary>
    string Hash(string password);

    /// <summary>Проверяет пароль по сохранённому хешу.</summary>
    bool Verify(string password, string hash);
}
