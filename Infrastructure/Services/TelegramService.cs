using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

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
            _logger.LogWarning("Telegram BotToken не настроен. Сообщение не отправлено.");
            return;
        }

        try
        {
            var payload = new { chat_id = telegramId, text = message, parse_mode = "HTML" };
            var response = await _httpClient.PostAsJsonAsync("sendMessage", payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Telegram sendMessage вернул {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке Telegram уведомления пользователю {TelegramId}", telegramId);
        }
    }

    public async Task<bool> SendOtpAsync(string telegramId, string otp, CancellationToken cancellationToken = default)
    {
        await SendMessageAsync(telegramId,
            $"🔐 Ваш код подтверждения для входа в ЗнаниеЗаЗнание:\n<b>{otp}</b>\n\nКод действителен 5 минут.",
            cancellationToken);
        return true;
    }
}
