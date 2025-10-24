namespace SoftwareCenter.Api.CatalogItems.Models;

public record CatalogItemDeleteModel
{
	public string Name { get; init; } = string.Empty;

	public string Description { get; init; } = string.Empty;
}
