using MediatR;

namespace Application.Features.Deals.Queries.GetDealById;

public record GetDealByIdQuery(Guid DealID, Guid RequestingAccountID) : IRequest<DealDetailDto>;

public record DealDetailDto(
    Guid DealID,
    Guid ApplicationID,
    Guid InitiatorID,
    string InitiatorName,
    Guid PartnerID,
    string PartnerName,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? OfferTitle,
    string? RequestTitle,
    List<ReviewDto> Reviews);

public record ReviewDto(
    Guid ReviewID,
    Guid AuthorID,
    string AuthorName,
    Guid TargetID,
    string TargetName,
    int Rating,
    string? Comment,
    DateTime CreatedAt);
