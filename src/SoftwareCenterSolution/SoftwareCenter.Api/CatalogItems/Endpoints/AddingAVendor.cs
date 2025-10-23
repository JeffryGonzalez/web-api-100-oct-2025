using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class AddCatalogItem
{
    public static async Task<Results<Created<CatalogItemResponseModel>, NotFound<string>, Conflict<string>, UnauthorizedHttpResult, ValidationProblem>> Handle(
        CatalogItemCreateModel request,
        IDocumentSession session,
        Guid vendorId,
        HttpContext httpContext,
        CatalogItemCreateModelValidator validator
    )
    {
        var user = httpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return TypedResults.Unauthorized();
        }

        // Validate the model
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        // Check if vendor exists
        var vendorExists = await session.Query<VendorEntity>().AnyAsync(v => v.Id == vendorId);
        if (!vendorExists)
        {
            return TypedResults.NotFound("Vendor not found");
        }

        // Check for uniqueness of name within this vendor
        var nameExists = await session.Query<CatalogItem>()
            .Where(c => c.VendorId == vendorId && c.Name == request.Name)
            .AnyAsync();

        if (nameExists)
        {
            return TypedResults.Conflict($"A catalog item with the name '{request.Name}' already exists for this vendor");
        }

        var entity = request.MapToEntity(vendorId, user.Identity?.Name ?? "Unknown");

        session.Store(entity);
        await session.SaveChangesAsync();
        
        var response = entity.MapToResponse();
        return TypedResults.Created($"/vendors/{vendorId}/catalog/{entity.Id}", response);
    }
}
