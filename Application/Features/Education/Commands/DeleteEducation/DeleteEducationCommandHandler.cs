using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Education.Commands.DeleteEducation;

public class DeleteEducationCommandHandler : IRequestHandler<DeleteEducationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEducationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
    {
        var education = await _context.Educations
            .FirstOrDefaultAsync(e => e.EducationID == request.EducationID, cancellationToken);

        if (education is null)
            throw new NotFoundException(nameof(Domain.Entities.Education), request.EducationID);

        if (education.AccountID != request.AccountID)
            throw new UnauthorizedAccessException("Нет доступа к удалению этой записи.");

        _context.Educations.Remove(education);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
