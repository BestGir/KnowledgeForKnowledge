using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserSkills.Commands.AddUserSkill;

public class AddUserSkillCommandHandler : IRequestHandler<AddUserSkillCommand>
{
    private readonly IApplicationDbContext _context;

    public AddUserSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AddUserSkillCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.UserSkills
            .AnyAsync(us => us.AccountID == request.AccountID && us.SkillID == request.SkillID, cancellationToken);

        if (exists) return; // Уже добавлен

        _context.UserSkills.Add(new UserSkill
        {
            AccountID = request.AccountID,
            SkillID = request.SkillID,
            SkillLevel = request.Level,
            IsVerified = false
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
