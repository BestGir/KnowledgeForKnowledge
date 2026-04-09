using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Accounts.Commands.DeactivateAccount;

public class DeactivateAccountCommandHandler : IRequestHandler<DeactivateAccountCommand>
{
    private readonly IApplicationDbContext _context;

    public DeactivateAccountCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsAdmin && request.RequestingAccountID != request.TargetAccountID)
            throw new UnauthorizedAccessException("Недостаточно прав.");

        var account = await _context.Accounts.FindAsync(
            new object[] { request.TargetAccountID }, cancellationToken);

        if (account is null)
            throw new NotFoundException(nameof(Domain.Entities.Account), request.TargetAccountID);

        account.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
