using MediatR;

namespace Application.Features.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(Guid AccountID, string? TelegramID) : IRequest;
