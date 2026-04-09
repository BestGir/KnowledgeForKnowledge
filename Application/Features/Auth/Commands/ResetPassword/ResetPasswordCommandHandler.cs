using Application.Common.Interfaces;
using Application.Features.Auth.Commands.VerifyOtp;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private const int MaxAttempts = 5;

    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMemoryCache _cache;

    public ResetPasswordCommandHandler(
        IApplicationDbContext context, IPasswordHasher passwordHasher, IMemoryCache cache)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _cache = cache;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            throw new InvalidOperationException("Новый пароль должен содержать не менее 8 символов.");

        var cacheKey = $"reset:{request.SessionId}";

        if (!_cache.TryGetValue(cacheKey, out OtpSession? session) || session is null)
            throw new UnauthorizedAccessException("Сессия сброса не найдена или истекла.");

        if (session.Attempts >= MaxAttempts)
        {
            _cache.Remove(cacheKey);
            throw new UnauthorizedAccessException("Превышено число попыток. Запросите сброс заново.");
        }

        if (session.Code != request.Code)
        {
            _cache.Set(cacheKey, session with { Attempts = session.Attempts + 1 }, TimeSpan.FromMinutes(10));
            throw new UnauthorizedAccessException("Неверный код.");
        }

        _cache.Remove(cacheKey);

        var account = await _context.Accounts.FindAsync(new object[] { session.AccountId }, cancellationToken);
        if (account is null)
            throw new UnauthorizedAccessException("Аккаунт не найден.");

        account.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        account.FailedLoginAttempts = 0;
        account.LockoutUntil = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
