using SoftwareCenter.Api.CatalogItems.Endpoints;

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

        var group = app.MapGroup("vendors"); // .RequireAuthorization();

        group.MapGet("/{vendorId:guid}/catalog", GetAllCatalogItemsForVendor.Handle);
        return app;
    }
}
