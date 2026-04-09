using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Skills.Commands.DeleteSkill;

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSkillCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await _context.SkillsCatalog
            .FirstOrDefaultAsync(s => s.SkillID == request.SkillID, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.SkillsCatalog), request.SkillID);

        // Проверяем, нет ли активных предложений или запросов с этим навыком
        var hasOffers = await _context.SkillOffers
            .AnyAsync(o => o.SkillID == request.SkillID && o.IsActive, cancellationToken);

        if (hasOffers)
            throw new InvalidOperationException(
                "Нельзя удалить навык: существуют активные предложения с ним. Сначала деактивируйте их.");

        _context.SkillsCatalog.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
