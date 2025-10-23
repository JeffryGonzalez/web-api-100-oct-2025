using Marten;
using SoftwareCenter.Api.CatalogItems.Endpoints;
using SoftwareCenter.Api.CatalogItems.Entities;

namespace SoftwareCenter.Api.CatalogItems;

public static class Extensions
{
    public static IServiceCollection AddCatalogItems(this IServiceCollection services)
    {
        // add feature specific services here
        return services;
    }

    public static WebApplication MapCatalogItems(this WebApplication app)
    {
        app.MapGet("catalog-items", async (IDocumentSession session) =>
        {
            var catalog = await session.Query<CatalogItem>().ToListAsync();
            return catalog;
        });
        // whatever I map as part of this group is going to be at "/vendors";
        var group = app.MapGroup("vendors");
        // /vendors/934893489384/catalog
        group.MapGet("/{vendorId:guid}/catalog", GetAllCatalogItemsForVendor.Handle);
        group.MapPost("/{vendorId:guid}/catalog", AddingAVendor.Handle);
        return app;
    }
}
