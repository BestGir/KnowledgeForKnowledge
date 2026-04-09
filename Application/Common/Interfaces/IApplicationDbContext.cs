using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<SkillsCatalog> SkillsCatalog { get; }
    DbSet<UserSkill> UserSkills { get; }
    DbSet<Education> Educations { get; }
    DbSet<Proof> Proofs { get; }
    DbSet<SkillOffer> SkillOffers { get; }
    DbSet<SkillRequest> SkillRequests { get; }
    DbSet<VerificationRequest> VerificationRequests { get; }
    DbSet<Domain.Entities.Application> Applications { get; }
    DbSet<Deal> Deals { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
