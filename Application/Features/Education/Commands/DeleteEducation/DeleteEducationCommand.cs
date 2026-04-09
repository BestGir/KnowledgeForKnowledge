using MediatR;

namespace Application.Features.Education.Commands.DeleteEducation;

public record DeleteEducationCommand(Guid EducationID, Guid AccountID) : IRequest;
