using System.Security.Claims;
using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class AddingACatalogItem
{
    public static async Task<Results<Ok<CatalogItem>, NotFound<string>, BadRequest<string>> Handle(
        CatalogItemCreateModel request,
        IDocumentSession session,
        IHttpContextAccessor context,
        Guid vendorId
        )
    {
        var doesVendorExist = await session.Query<VendorEntity>().AnyAsync(v => v.Id == vendorId);
        if (!doesVendorExist)
        {
            return TypedResults.NotFound("That vendor does not exist");
        }

        if (await session.Query<CatalogItem>()
                .Where(v => v.VendorId == vendorId)
                .AnyAsync(i => i.Name == request.Name))
        {
            return TypedResults.BadRequest("An item with that name already exists for this vendor");
        }

        var entity = new CatalogItem
        {
            Id = Guid.NewGuid(),
            VendorId = vendorId,
            Name = request.Name,
            Description = request.Description,
            CreatedBy = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
        }; // Todo: Mapper would be nice, right?

        session.Store(entity);
        await session.SaveChangesAsync();
        return TypedResults.Ok(entity); // Make a response model for this.
    }
}
