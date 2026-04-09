using MediatR;

namespace Application.Features.Auth.Commands.Login;

public record LoginCommand(string Login, string Password) : IRequest<LoginResult>;

/// <summary>
/// Если RequiresOtp = true — токен пустой, нужно пройти VerifyOtp с SessionId.
/// Если RequiresOtp = false — токен сразу готов.
/// </summary>
public record LoginResult(string Token, Guid AccountId, bool IsAdmin, bool RequiresOtp = false, string? SessionId = null);
