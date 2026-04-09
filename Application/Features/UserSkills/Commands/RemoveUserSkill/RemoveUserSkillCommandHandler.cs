using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserSkills.Commands.RemoveUserSkill;

public class RemoveUserSkillCommandHandler : IRequestHandler<RemoveUserSkillCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveUserSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveUserSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _context.UserSkills
            .FirstOrDefaultAsync(us => us.AccountID == request.AccountID && us.SkillID == request.SkillID, cancellationToken);

        if (skill is null)
            throw new NotFoundException("UserSkill", $"{request.AccountID}/{request.SkillID}");

        _context.UserSkills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
