using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Login) : IRequest<ForgotPasswordResult>;

public record ForgotPasswordResult(string SessionId);
