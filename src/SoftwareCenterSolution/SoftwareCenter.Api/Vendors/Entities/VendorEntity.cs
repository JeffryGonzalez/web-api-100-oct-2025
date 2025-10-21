using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors.Entities;

// what I'm actually storing in the database
public class VendorEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VendorPointOfContact PointOfContact { get; set; } = new();
    // who created this??
}



