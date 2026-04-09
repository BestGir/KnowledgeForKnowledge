using MediatR;

namespace Application.Features.UserProfiles.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid AccountID, Guid? RequestingAccountID = null) : IRequest<UserProfileDto>;

public record UserProfileDto(
    Guid AccountID,
    string FullName,
    DateTime? DateOfBirth,
    string? PhotoURL,
    string? ContactInfo,
    string? Description,
    bool IsActive,
    DateTime? LastSeenOnline,
    bool HasProfile
);
