using MediatR;

namespace Application.Features.Deals.Queries.GetPublicDeals;

public record GetPublicDealsQuery(Guid TargetAccountID, int Page = 1, int PageSize = 20)
    : IRequest<GetPublicDealsResult>;

public record PublicDealDto(
    Guid DealID,
    string PartnerName,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? OfferTitle,
    string? RequestTitle);

public record GetPublicDealsResult(
    List<PublicDealDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
