using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string SessionId, string Code, string NewPassword) : IRequest;
