namespace SoftwareCenter.Api.CatalogItems.Entities;

public class CatalogItemEntity
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }= string.Empty;
}
