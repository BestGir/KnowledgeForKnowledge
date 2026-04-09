using MediatR;

namespace Application.Features.Proofs.Commands.CreateProof;

public record CreateProofCommand(Guid AccountID, Guid? SkillID, string FileURL) : IRequest<Guid>;
