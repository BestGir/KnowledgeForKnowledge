using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Accounts.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountID == request.AccountID, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountID);

        if (!_passwordHasher.Verify(request.CurrentPassword, account.PasswordHash))
            throw new UnauthorizedAccessException("Неверный текущий пароль.");

        if (request.NewPassword.Length < 6)
            throw new InvalidOperationException("Новый пароль должен содержать не менее 6 символов.");

        account.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
