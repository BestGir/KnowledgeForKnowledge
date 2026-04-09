using System.Security.Cryptography;
using Application.Common.Interfaces;
using Application.Features.Auth.Commands.VerifyOtp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ITelegramService _telegram;
    private readonly IMemoryCache _cache;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context, ITelegramService telegram, IMemoryCache cache)
    {
        _context = context;
        _telegram = telegram;
        _cache = cache;
    }

    public async Task<ForgotPasswordResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Login == request.Login, cancellationToken);

        // Не раскрываем, существует ли аккаунт
        if (account is null || string.IsNullOrEmpty(account.TelegramID))
            return new ForgotPasswordResult(string.Empty);

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var sessionId = Guid.NewGuid().ToString("N");

        _cache.Set($"reset:{sessionId}", new OtpSession(account.AccountID, code, 0),
            TimeSpan.FromMinutes(10));

        await _telegram.SendMessageAsync(account.TelegramID,
            $"🔑 Код сброса пароля: *{code}*\nДействителен 10 минут.",
            cancellationToken);

        return new ForgotPasswordResult(sessionId);
    }
}
