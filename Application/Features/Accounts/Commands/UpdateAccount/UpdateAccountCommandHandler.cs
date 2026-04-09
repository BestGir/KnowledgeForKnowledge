using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountID == request.AccountID, cancellationToken);

        if (account is null)
            throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountID);

        account.TelegramID = request.TelegramID;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
