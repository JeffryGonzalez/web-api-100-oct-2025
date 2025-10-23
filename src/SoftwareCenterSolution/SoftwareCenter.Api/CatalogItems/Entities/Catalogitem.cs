namespace SoftwareCenter.Api.CatalogItems.Entities;

public class CatalogItem
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }= string.Empty;
}
