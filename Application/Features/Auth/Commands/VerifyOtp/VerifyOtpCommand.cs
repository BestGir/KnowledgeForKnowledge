using MediatR;

namespace Application.Features.Auth.Commands.VerifyOtp;

public record VerifyOtpCommand(string SessionId, string Code) : IRequest<VerifyOtpResult>;

public record VerifyOtpResult(string Token, Guid AccountId, bool IsAdmin);
