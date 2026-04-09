using MediatR;

namespace Application.Features.Education.Commands.AddEducation;

public record AddEducationCommand(Guid AccountID, string InstitutionName, string? DegreeField, int? YearCompleted) : IRequest<Guid>;
