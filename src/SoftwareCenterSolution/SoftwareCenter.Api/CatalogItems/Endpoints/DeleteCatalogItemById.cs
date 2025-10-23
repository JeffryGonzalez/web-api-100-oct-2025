using System.Security.Claims;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.Vendors.Entities;
using Spectre.Console.Rendering;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class DeleteCatalogItemById
{
    public static async Task<Results<Ok, NotFound<string>>> Handle(
        Guid vendorId,
        Guid itemId,
        IDocumentSession session,
        IHttpContextAccessor context
        )
    {
        var doesVendorExist = await session.Query<VendorEntity>().AnyAsync(v => v.Id == vendorId);
        if (doesVendorExist)
        {
            var item = await session.Query<CatalogItem>()
                .Where(v => v.VendorId == vendorId)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item != null)
            {
                if (IsUserManager(context) || IsUserThatCreatedItem(context, item))
                {
                    session.Delete<CatalogItem>(item.Id);
                    await session.SaveChangesAsync();
                }
                else
                {
                    TypedResults.Forbid();
                }
            }

            return TypedResults.Ok();
        }
        else
        {
            return TypedResults.NotFound("No Vendor With That Id");
        }
    }

    private static bool IsUserManager(IHttpContextAccessor context)
    {
        return context.HttpContext.User.IsInRole("Manager");
    }

    private static bool IsUserThatCreatedItem(IHttpContextAccessor context, CatalogItem item)
    {
        var userSub = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        return string.Equals(userSub, item.CreatedBy);
    }
}
