using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Auth.Commands.VerifyOtp;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResult>
{
    private const int MaxOtpAttempts = 5;

    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IMemoryCache _cache;

    public VerifyOtpCommandHandler(IApplicationDbContext context, IJwtService jwtService, IMemoryCache cache)
    {
        _context = context;
        _jwtService = jwtService;
        _cache = cache;
    }

    public async Task<VerifyOtpResult> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"otp:{request.SessionId}";

        if (!_cache.TryGetValue(cacheKey, out OtpSession? session) || session is null)
            throw new UnauthorizedAccessException("Сессия OTP не найдена или истекла.");

        // Rate limit: не более MaxOtpAttempts попыток на сессию
        if (session.Attempts >= MaxOtpAttempts)
        {
            _cache.Remove(cacheKey);
            throw new UnauthorizedAccessException("Превышено число попыток. Войдите заново.");
        }

        if (session.Code != request.Code)
        {
            // Увеличиваем счётчик попыток
            var updated = session with { Attempts = session.Attempts + 1 };
            _cache.Set(cacheKey, updated, TimeSpan.FromMinutes(5));
            throw new UnauthorizedAccessException("Неверный код подтверждения.");
        }

        _cache.Remove(cacheKey);

        var account = await _context.Accounts.FindAsync(new object[] { session.AccountId }, cancellationToken);
        if (account is null)
            throw new UnauthorizedAccessException("Аккаунт не найден.");

        var token = _jwtService.GenerateToken(account);
        return new VerifyOtpResult(token, account.AccountID, account.IsAdmin);
    }
}

public record OtpSession(Guid AccountId, string Code, int Attempts);
