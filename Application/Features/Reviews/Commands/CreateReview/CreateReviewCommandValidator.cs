using FluentValidation;

namespace Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.DealID)
            .NotEmpty().WithMessage("DealID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.")
            .When(x => x.Comment != null);
    }
}
