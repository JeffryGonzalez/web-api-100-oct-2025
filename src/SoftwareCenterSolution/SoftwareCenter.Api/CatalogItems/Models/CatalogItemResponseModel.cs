namespace SoftwareCenter.Api.CatalogItems.Models;

public record CatalogItemResponseModel
{
    public Guid Id { get; init; }
    public Guid VendorId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
