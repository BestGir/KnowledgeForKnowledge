using MediatR;

namespace Application.Features.Proofs.Queries.GetProofs;

public record GetProofsQuery(Guid AccountID) : IRequest<List<ProofDto>>;

public record ProofDto(Guid ProofID, Guid? SkillID, string? SkillName, string FileURL, bool IsVerified);
