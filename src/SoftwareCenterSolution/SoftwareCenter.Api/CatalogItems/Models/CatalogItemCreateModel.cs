using Marten;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using SoftwareCenter.Api.CatalogItems.Entities;

namespace SoftwareCenter.Api.CatalogItems.Models;

public record CatalogItemCreateModel
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}
public class CatalogItemCreateModelValidator : AbstractValidator<CatalogItemCreateModel>
{
    private readonly IDocumentSession _session;

    public CatalogItemCreateModelValidator(IDocumentSession session)
    {
        _session = session;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(500);
        RuleFor(x => x)
            .MustAsync(async (model, cancellation) => await IsUniqueCatalogItem(model))
            .WithMessage("A catalog item with the same name and description already exists.");
    }

    private async Task<bool> IsUniqueCatalogItem(CatalogItemCreateModel model)
    {
        var existingItem = await _session.Query<CatalogItem>()
            .FirstOrDefaultAsync(ci => ci.Name == model.Name && ci.Description == model.Description);

        return existingItem == null;
    }
}
