using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class RemoveCatalogItem
{
    public static async Task<Results<NoContent, NotFound<string>, UnauthorizedHttpResult, ForbidHttpResult>> Handle(
        Guid vendorId,
        Guid catalogItemId,
        IDocumentSession session,
        HttpContext httpContext
    )
    {
        var user = httpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return TypedResults.Unauthorized();
        }

        // Find the catalog item
        var catalogItem = await session.Query<CatalogItem>()
            .Where(c => c.Id == catalogItemId && c.VendorId == vendorId)
            .SingleOrDefaultAsync();

        if (catalogItem is null)
        {
            return TypedResults.NotFound("Catalog item not found");
        }

        // Check authorization: either a Software Center Manager OR the creator
        var isSoftwareCenterManager = user.IsInRole("SoftwareCenter") && user.IsInRole("Manager");
        var isCreator = catalogItem.CreatedBy == (user.Identity?.Name ?? string.Empty);

        if (!isSoftwareCenterManager && !isCreator)
        {
            return TypedResults.Forbid();
        }

        // Delete the item
        session.Delete(catalogItem);
        await session.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
