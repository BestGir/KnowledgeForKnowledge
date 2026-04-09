using Application.Common.Models;
using MediatR;

namespace Application.Features.Accounts.Queries.GetAccounts;

/// <summary>Список аккаунтов с пагинацией (только Admin).</summary>
public record GetAccountsQuery(
    string? Search,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<AccountListDto>>;

public record AccountListDto(
    Guid AccountID,
    string Login,
    string? TelegramID,
    bool IsAdmin,
    bool HasProfile,
    string? FullName,
    DateTime CreatedAt
);
