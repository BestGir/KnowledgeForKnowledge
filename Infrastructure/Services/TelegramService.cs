using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _httpClient;
    private readonly string _botToken;
    private readonly ILogger<TelegramService> _logger;

    public TelegramService(IConfiguration configuration, ILogger<TelegramService> logger)
    {
        _logger = logger;
        _botToken = configuration["Telegram:BotToken"] ?? string.Empty;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://api.telegram.org/bot{_botToken}/")
        };
    }

    public async Task SendMessageAsync(string telegramId, string message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_botToken))
        {
            _logger.LogWarning("Telegram BotToken is not configured. Message is skipped.");
            return;
        }

        if (string.IsNullOrWhiteSpace(telegramId))
        {
            _logger.LogWarning("Telegram chat id is empty. Message is skipped.");
            return;
        }

        try
        {
            var payload = new
            {
                chat_id = telegramId.Trim(),
                text = message
            };

            using var response = await _httpClient.PostAsJsonAsync("sendMessage", payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var details = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Telegram sendMessage returned {StatusCode}. Response: {ResponseBody}",
                    response.StatusCode,
                    details);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending Telegram message to {TelegramId}", telegramId);
        }
    }

    public async Task<bool> SendOtpAsync(string telegramId, string otp, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_botToken))
        {
            _logger.LogWarning("Telegram BotToken is not configured. OTP message is skipped.");
            return false;
        }

        try
        {
            var payload = new
            {
                chat_id = telegramId,
                text = $"Ваш код подтверждения для входа:\n<b>{otp}</b>\n\nКод действителен 5 минут.",
                parse_mode = "HTML"
            };

            using var response = await _httpClient.PostAsJsonAsync("sendMessage", payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Telegram sendMessage for OTP returned {StatusCode}", response.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending OTP to Telegram user {TelegramId}", telegramId);
            return false;
        }
    }

    public async Task<string?> TryResolveChatIdByStartTokenAsync(string linkToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_botToken) || string.IsNullOrWhiteSpace(linkToken))
        {
            return null;
        }

        try
        {
            using var response = await _httpClient.GetAsync("getUpdates?limit=100&timeout=0", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Telegram getUpdates returned {StatusCode}", response.StatusCode);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            if (!document.RootElement.TryGetProperty("ok", out var okElement) || !okElement.GetBoolean())
            {
                return null;
            }

            if (!document.RootElement.TryGetProperty("result", out var resultElement) ||
                resultElement.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var updates = resultElement.EnumerateArray().ToArray();
            var normalizedToken = linkToken.Trim().ToUpperInvariant();

            for (var index = updates.Length - 1; index >= 0; index--)
            {
                var update = updates[index];
                if (!update.TryGetProperty("message", out var messageElement))
                {
                    continue;
                }

                if (!messageElement.TryGetProperty("text", out var textElement) ||
                    textElement.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                if (!TryExtractStartToken(textElement.GetString(), out var tokenFromMessage) ||
                    !string.Equals(tokenFromMessage, normalizedToken, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (messageElement.TryGetProperty("chat", out var chatElement) &&
                    chatElement.TryGetProperty("id", out var chatIdElement) &&
                    chatIdElement.TryGetInt64(out var chatId))
                {
                    return chatId.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking Telegram updates for link token.");
        }

        return null;
    }

    private static bool TryExtractStartToken(string? text, out string token)
    {
        token = string.Empty;

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 2 || !parts[0].StartsWith("/start", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        token = parts[1].Trim().ToUpperInvariant();
        return !string.IsNullOrWhiteSpace(token);
    }
}
