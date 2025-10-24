using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.CatalogItems.Services;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class AddingACatalogItem
{
    public static async Task<Results<Ok<CatalogItemDetails>, NotFound<string>>> Handle(
        CatalogItemCreateModel request,
       MartenPostgresCatalogManager catalogManager,
        Guid vendorId
        )
    {
        var (results, response) = await catalogManager.AddCatalogItemAsync(request, vendorId);

        return results switch
        {
            ApiResults.NotFound => TypedResults.NotFound("No Vendor With That Id"),
            ApiResults.Succceded => TypedResults.Ok(response),
            _ => throw new NotImplementedException()
        };
       
    }
}
