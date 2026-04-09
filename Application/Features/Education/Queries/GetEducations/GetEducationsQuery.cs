using MediatR;

namespace Application.Features.Education.Queries.GetEducations;

public record GetEducationsQuery(Guid AccountID) : IRequest<List<EducationDto>>;

public record EducationDto(
    Guid EducationID,
    string InstitutionName,
    string? DegreeField,
    int? YearCompleted
);
