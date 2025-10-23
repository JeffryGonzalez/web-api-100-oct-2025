using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class AddingAVendor
{
    public static async Task<Ok<CatalogItem>> Handle(
        CatalogItemCreateModel request,
        IDocumentSession session,
        Guid vendorId
        )
    {
        // todo: Probably should check if that vendor exists.
        // validate...
        var entity = new CatalogItem
        {
            Id = Guid.NewGuid(),
            VendorId = vendorId,
            Name = request.Name,
            Description = request.Description,
        }; // Todo: Mapper would be nice, right?

        session.Store(entity);
        await session.SaveChangesAsync();
        return TypedResults.Ok(entity); // Make a response model for this.
    }
}
