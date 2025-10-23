using FluentValidation;

namespace SoftwareCenter.Api.CatalogItems.Models;

public record CatalogItemCreateModel
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}

public class CatalogItemCreateModelValidator : AbstractValidator<CatalogItemCreateModel>
{
    public CatalogItemCreateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Catalog item name is required")
            .MinimumLength(3)
            .WithMessage("Catalog item name must be at least 3 characters")
            .MaximumLength(100)
            .WithMessage("Catalog item name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MinimumLength(10)
            .WithMessage("Description must be at least 10 characters")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");
    }
}

// todo - add validators.
