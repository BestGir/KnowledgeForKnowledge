using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SkillRequests.Commands.DeleteSkillRequest;

public class DeleteSkillRequestCommandHandler : IRequestHandler<DeleteSkillRequestCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSkillRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSkillRequestCommand request, CancellationToken cancellationToken)
    {
        var skillRequest = await _context.SkillRequests
            .FirstOrDefaultAsync(r => r.RequestID == request.RequestID, cancellationToken);

        if (skillRequest is null)
            throw new NotFoundException(nameof(Domain.Entities.SkillRequest), request.RequestID);

        if (skillRequest.AccountID != request.AccountID)
            throw new UnauthorizedAccessException("Нельзя удалить чужой запрос.");

        _context.SkillRequests.Remove(skillRequest);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
