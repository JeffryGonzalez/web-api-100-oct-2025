using Riok.Mapperly.Abstractions;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.Vendors.Models;


public class CollectionResponseModel<T>
{
    public IList<T> Data { get; set; } = new List<T>();
}

public record VendorSummaryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

