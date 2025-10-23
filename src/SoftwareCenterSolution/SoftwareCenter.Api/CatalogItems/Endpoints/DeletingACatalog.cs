using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class DeletingACatalog
{
    public static async Task<Results<Ok<CatalogItemEntity>, NotFound<string>>> Handle(
        Guid vendorId,
        Guid catalogItemId,
        IDocumentSession session
        ) 
    {
        var doesCatalogExist = await session.Query<CatalogItemEntity>().AnyAsync(c => c.VendorId == vendorId && c.Id == catalogItemId);
        if (doesCatalogExist)
        {
            var response = await session.Query<CatalogItemEntity>()
                .Where(c => c.VendorId == vendorId && c.Id == catalogItemId)
                .FirstOrDefaultAsync();
            session.DeleteWhere<CatalogItemEntity>(c => c.VendorId == vendorId && c.Id == catalogItemId);
            return TypedResults.Ok(response);
        }
        else
        {
            return TypedResults.NotFound("No Catalog Item With That Id");
        }
    }
}
