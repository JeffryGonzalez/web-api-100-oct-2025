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

//public static class VendorSummaryItemMappers
//{
//    public static VendorSummaryItem MapFromEntity(this VendorEntity entity)
//    {
//        return new VendorSummaryItem
//        {
//            Id = entity.Id,
//            Name = entity.Name,
//        };
//    }
//}

[Mapper]
public static partial class VendorMappers
{
    public static partial IQueryable<VendorSummaryItem> ProjectToSummary(this IQueryable<VendorEntity> q);
    public static partial VendorSummaryItem MapFromEntity(this VendorEntity entity);
}