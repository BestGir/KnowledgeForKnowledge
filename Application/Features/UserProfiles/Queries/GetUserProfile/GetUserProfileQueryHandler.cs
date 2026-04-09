using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserProfiles.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // Check if account exists
        var account = await _context.Accounts
            .Where(a => a.AccountID == request.AccountID)
            .Select(a => new { a.AccountID, a.Login, a.IsActive, a.IsAdmin })
            .FirstOrDefaultAsync(cancellationToken);

        if (account is null)
            throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountID);

        var canSeeContactInfo = request.RequestingAccountID == request.AccountID
                                || account.IsAdmin
                                || (request.RequestingAccountID.HasValue && await _context.Accounts
                                    .Where(a => a.AccountID == request.RequestingAccountID.Value)
                                    .Select(a => a.IsAdmin)
                                    .FirstOrDefaultAsync(cancellationToken));

        var profile = await _context.UserProfiles
            .Where(p => p.AccountID == request.AccountID)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile is null)
        {
            // Return partial profile built from Account — no 404
            return new UserProfileDto(
                account.AccountID,
                account.Login,
                null,
                null,
                null,
                null,
                account.IsActive,
                null,
                HasProfile: false);
        }

        return new UserProfileDto(
            profile.AccountID,
            profile.FullName,
            profile.DateOfBirth,
            profile.PhotoURL,
            canSeeContactInfo ? profile.ContactInfo : null,
            profile.Description,
            profile.IsActive,
            profile.LastSeenOnline,
            HasProfile: true);
    }
}
