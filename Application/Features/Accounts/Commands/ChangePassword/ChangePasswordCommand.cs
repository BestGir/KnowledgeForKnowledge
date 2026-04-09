using MediatR;

namespace Application.Features.Accounts.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid AccountID,
    string CurrentPassword,
    string NewPassword
) : IRequest;
