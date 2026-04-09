using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillRequests.Commands.UpdateSkillRequestStatus;

public class UpdateSkillRequestStatusCommandHandler : IRequestHandler<UpdateSkillRequestStatusCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSkillRequestStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSkillRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var skillRequest = await _context.SkillRequests
            .FirstOrDefaultAsync(r => r.RequestID == request.RequestID, cancellationToken);

        if (skillRequest is null)
            throw new NotFoundException(nameof(Domain.Entities.SkillRequest), request.RequestID);

        if (skillRequest.AccountID != request.AccountID)
            throw new UnauthorizedAccessException("Нет доступа к изменению этого запроса.");

        skillRequest.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
