namespace Application.Common.Interfaces;

public interface ITelegramService
{
    Task SendMessageAsync(string telegramId, string message, CancellationToken cancellationToken = default);
    Task<bool> SendOtpAsync(string telegramId, string otp, CancellationToken cancellationToken = default);
    Task<string?> TryResolveChatIdByStartTokenAsync(string linkToken, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);
}
