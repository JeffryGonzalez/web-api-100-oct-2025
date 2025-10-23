using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class GetAllCatalogItemsForVendor
{
    public static async Task<Ok<IReadOnlyList<CatalogItem>>> Handle(
        Guid vendorId,
        IDocumentSession session
        )
    {
        var response = await session.Query<CatalogItem>()
            .Where(v => v.VendorId == vendorId)
            .ToListAsync();
        return  TypedResults.Ok(response );
    }
}
