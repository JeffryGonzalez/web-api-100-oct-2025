namespace SoftwareCenter.Api.CatalogItems.Models;

public record CatalogItemCreateModel
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}

// todo - add validators.
