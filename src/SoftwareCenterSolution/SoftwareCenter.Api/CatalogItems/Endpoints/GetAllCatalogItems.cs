using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class GetAllCatalogItemsForVendor
{
    public static async Task<Results<Ok<IReadOnlyList<CatalogItemDetails>>,NotFound<string>>> Handle(
        Guid vendorId,
        IDocumentSession session
        )
    {
        var doesVendorExist = await session.Query<VendorEntity>().AnyAsync(v => v.Id == vendorId);
        if (doesVendorExist)
        {
            var response = await session.Query<CatalogItemEntity>()
                .Where(v => v.VendorId == vendorId)
                .ProjectTo()
                .ToListAsync();
            return TypedResults.Ok(response);
        } else
        {
            return TypedResults.NotFound("No Vendor With That Id");
        }
    }
}
