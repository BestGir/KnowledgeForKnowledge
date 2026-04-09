using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.VerificationRequests.Commands.SubmitVerificationRequest;

public class SubmitVerificationRequestCommandHandler
    : IRequestHandler<SubmitVerificationRequestCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public SubmitVerificationRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        SubmitVerificationRequestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new VerificationRequest
        {
            RequestID   = Guid.NewGuid(),
            AccountID   = request.AccountID,
            RequestType = request.RequestType,
            ProofID     = request.ProofID,
            Status      = VerificationStatus.Pending,
            CreatedAt   = DateTime.UtcNow
        };

        _context.VerificationRequests.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.RequestID;
    }
}
