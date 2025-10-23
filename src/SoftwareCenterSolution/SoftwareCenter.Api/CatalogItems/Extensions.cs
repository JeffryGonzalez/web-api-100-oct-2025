using Marten;
using SoftwareCenter.Api.CatalogItems.Endpoints;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;

namespace SoftwareCenter.Api.CatalogItems;

public static class Extensions
{
    public static IServiceCollection AddCatalogItems(this IServiceCollection services)
    {
        services.AddScoped<CatalogItemCreateModelValidator>();
        return services;
    }

    public static WebApplication MapCatalogItems(this WebApplication app)
    {
        app.MapGet("catalog-items", async (IDocumentSession session) =>
        {
            var catalog = await session.Query<CatalogItem>().ToListAsync();
            return catalog;
        });
        
        var group = app.MapGroup("vendors");
        
        group.MapGet("/{vendorId:guid}/catalog", GetAllCatalogItemsForVendor.Handle);
        
        group.MapPost("/{vendorId:guid}/catalog", AddCatalogItem.Handle)
            .RequireAuthorization();
        
        group.MapDelete("/{vendorId:guid}/catalog/{catalogItemId:guid}", RemoveCatalogItem.Handle)
            .RequireAuthorization();
        
        return app;
    }
}
