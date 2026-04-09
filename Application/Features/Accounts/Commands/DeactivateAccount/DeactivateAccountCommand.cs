using MediatR;

namespace Application.Features.Accounts.Commands.DeactivateAccount;

public record DeactivateAccountCommand(Guid TargetAccountID, Guid RequestingAccountID, bool IsAdmin) : IRequest;
