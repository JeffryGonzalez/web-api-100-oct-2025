namespace SoftwareCenter.Api.Vendors.Models;

// what I am returning to the caller on the POST and the GET /vendors/{id}
public record VendorDetailsModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VendorPointOfContact PointOfContact { get; set; } = new();
}



