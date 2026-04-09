using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.VerifyOtp;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    /// <summary>
    /// Вход в систему.
    /// Если к аккаунту привязан Telegram — возвращает {requiresOtp: true, sessionId}.
    /// Иначе сразу возвращает JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Второй шаг 2FA: передать sessionId из /login и код из Telegram</summary>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(VerifyOtpResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Запросить сброс пароля через Telegram.
    /// Если к аккаунту привязан Telegram — отправляет OTP и возвращает sessionId.
    /// Ответ одинаков независимо от того, существует ли аккаунт (защита от перебора).
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await Mediator.Send(new ForgotPasswordCommand(request.Login));
        return Ok(new { sessionId = result.SessionId });
    }

    /// <summary>Сбросить пароль с кодом из Telegram</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}

public record ForgotPasswordRequest(string Login);
