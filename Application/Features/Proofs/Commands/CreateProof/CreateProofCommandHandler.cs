using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Proofs.Commands.CreateProof;

public class CreateProofCommandHandler : IRequestHandler<CreateProofCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProofCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProofCommand request, CancellationToken cancellationToken)
    {
        var proof = new Proof
        {
            ProofID = Guid.NewGuid(),
            AccountID = request.AccountID,
            SkillID = request.SkillID,
            FileURL = request.FileURL,
            IsVerified = false
        };

        _context.Proofs.Add(proof);
        await _context.SaveChangesAsync(cancellationToken);
        return proof.ProofID;
    }
}
