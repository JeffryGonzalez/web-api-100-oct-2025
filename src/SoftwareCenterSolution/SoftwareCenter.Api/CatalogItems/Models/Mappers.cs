using Riok.Mapperly.Abstractions;
using SoftwareCenter.Api.CatalogItems.Entities;

namespace SoftwareCenter.Api.CatalogItems.Models;

[Mapper]
public static partial class CatalogItemMappers
{
    public static partial CatalogItemResponseModel MapToResponse(this CatalogItem entity);
    
    public static partial IQueryable<CatalogItemResponseModel> ProjectToResponse(this IQueryable<CatalogItem> query);
}

public static class CatalogItemManualMappers
{
    public static CatalogItem MapToEntity(this CatalogItemCreateModel model, Guid vendorId, string createdBy)
    {
        return new CatalogItem
        {
            Id = Guid.NewGuid(),
            VendorId = vendorId,
            Name = model.Name,
            Description = model.Description,
            CreatedBy = createdBy
        };
    }
}
