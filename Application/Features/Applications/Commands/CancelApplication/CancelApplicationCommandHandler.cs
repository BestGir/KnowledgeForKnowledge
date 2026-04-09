using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Applications.Commands.CancelApplication;

public class CancelApplicationCommandHandler : IRequestHandler<CancelApplicationCommand>
{
    private readonly IApplicationDbContext _context;

    public CancelApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CancelApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.ApplicationID == request.ApplicationID, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Application), request.ApplicationID);

        if (application.ApplicantID != request.ApplicantID)
            throw new UnauthorizedAccessException("Нет доступа к этому отклику.");

        if (application.Status != ApplicationStatus.Pending)
            throw new InvalidOperationException("Нельзя отозвать уже обработанный отклик.");

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
