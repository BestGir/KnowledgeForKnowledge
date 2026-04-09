using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Education.Commands.AddEducation;

public class AddEducationCommandHandler : IRequestHandler<AddEducationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddEducationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AddEducationCommand request, CancellationToken cancellationToken)
    {
        var education = new Domain.Entities.Education
        {
            EducationID = Guid.NewGuid(),
            AccountID = request.AccountID,
            InstitutionName = request.InstitutionName,
            DegreeField = request.DegreeField,
            YearCompleted = request.YearCompleted
        };

        _context.Educations.Add(education);
        await _context.SaveChangesAsync(cancellationToken);
        return education.EducationID;
    }
}
