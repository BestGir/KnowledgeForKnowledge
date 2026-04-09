using System.Security.Claims;
using Application.Features.Reviews.Commands.CreateReview;
using Application.Features.Reviews.Queries.GetReviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : BaseController
{
    private Guid CurrentAccountId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Отзывы о пользователе</summary>
    [HttpGet("{accountId:guid}")]
    public async Task<IActionResult> GetReviews(
        Guid accountId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetReviewsQuery(accountId, page, pageSize));
        return Ok(result);
    }

    /// <summary>Оставить отзыв по завершённой сделке</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var id = await Mediator.Send(new CreateReviewCommand(
            request.DealID, CurrentAccountId, request.Rating, request.Comment));
        return Created($"/api/reviews", new { id });
    }
}

public record CreateReviewRequest(Guid DealID, int Rating, string? Comment);
