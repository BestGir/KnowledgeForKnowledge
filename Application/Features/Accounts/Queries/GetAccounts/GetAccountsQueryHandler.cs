using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Accounts.Queries.GetAccounts;

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, PagedResult<AccountListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAccountsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AccountListDto>> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Accounts
            .Include(a => a.UserProfile)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(a =>
                a.Login.ToLower().Contains(s) ||
                (a.UserProfile != null && a.UserProfile.FullName.ToLower().Contains(s)));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AccountListDto(
                a.AccountID,
                a.Login,
                a.TelegramID,
                a.IsAdmin,
                a.UserProfile != null,
                a.UserProfile != null ? a.UserProfile.FullName : null,
                a.CreatedAt))
            .ToListAsync(cancellationToken);

        return PagedResult<AccountListDto>.Create(items, total, request.Page, request.PageSize);
    }
}
