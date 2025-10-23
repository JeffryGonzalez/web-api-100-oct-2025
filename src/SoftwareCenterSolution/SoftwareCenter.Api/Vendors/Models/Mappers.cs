using Riok.Mapperly.Abstractions;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.Vendors.Models;


[Mapper]
public static partial class VendorMappers
{
    public static partial IQueryable<VendorSummaryItem> ProjectToSummary(this IQueryable<VendorEntity> q);

    [MapperIgnoreSource(nameof(VendorEntity.PointOfContact))]
    public static partial VendorSummaryItem MapFromEntity(this VendorEntity entity);

    [MapValue(nameof(VendorEntity.Id), Use = nameof(GetVendorId))]
    public static partial VendorEntity MapToEntity(this VendorCreateModel model, string createdBy);

    public static partial VendorDetailsModel MapToResponse(this VendorEntity entity);

    private static Guid GetVendorId() => Guid.NewGuid();
}