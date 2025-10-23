using System.Security.Claims;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class RemoveACatalogItem
{
    [Authorize]
    public static async Task<IResult> Handle(
        [FromBody] CatalogItemCreateModel request,
        [FromServices] IDocumentSession session,
        [FromRoute] Guid vendorId,
        HttpContext httpContext
    )
    {
        if (httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
        {
            return TypedResults.Unauthorized();
        }

        var userId = httpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        var userRoles = httpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var catalogItem = await session.Query<CatalogItem>()
            .FirstOrDefaultAsync(ci => ci.VendorId == vendorId && ci.Name == request.Name);

        if (catalogItem == null)
        {
            return TypedResults.NotFound(new { error = "Catalog item not found." });
        }

        session.Delete(catalogItem);
        await session.SaveChangesAsync();

        return TypedResults.Ok();
    }
}