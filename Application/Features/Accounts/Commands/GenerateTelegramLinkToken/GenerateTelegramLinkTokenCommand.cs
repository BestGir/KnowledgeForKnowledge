using MediatR;

namespace Application.Features.Accounts.Commands.GenerateTelegramLinkToken;

public record GenerateTelegramLinkTokenCommand(Guid AccountID) : IRequest<string>;
