using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.Commands.MarkNotificationsRead;

public class MarkNotificationsReadCommandHandler : IRequestHandler<MarkNotificationsReadCommand>
{
    private readonly IApplicationDbContext _context;

    public MarkNotificationsReadCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(MarkNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        if (request.NotificationID.HasValue)
        {
            var n = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationID == request.NotificationID.Value
                                       && x.AccountID == request.AccountID, cancellationToken);
            if (n is not null) n.IsRead = true;
        }
        else
        {
            await _context.Notifications
                .Where(x => x.AccountID == request.AccountID && !x.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true), cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
