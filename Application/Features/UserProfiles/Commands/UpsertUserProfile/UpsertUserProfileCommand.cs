using MediatR;

namespace Application.Features.UserProfiles.Commands.UpsertUserProfile;

public record UpsertUserProfileCommand(
    Guid AccountID,
    string FullName,
    DateTime? DateOfBirth,
    string? PhotoURL,
    string? ContactInfo,
    string? Description
) : IRequest;
