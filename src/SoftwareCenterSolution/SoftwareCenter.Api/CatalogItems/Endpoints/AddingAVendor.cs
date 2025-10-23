using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FluentValidation;
using JasperFx.CodeGeneration.Frames;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public class AddingAVendor()
{
    [Authorize(Policy = "software-center-manager")]
    public static async Task<Results<Ok<CatalogItem>, BadRequest<String>>> Handle(
        [FromBody] CatalogItemCreateModel request,
        [FromServices] IDocumentSession session,
        //[FromServices] IValidator<CatalogItemCreateModel> validator,
        [FromRoute] Guid vendorId,
        HttpContext httpContext)
    {
        // Validation almost works??
        //var validationResult = await validator.ValidateAsync(request);

        //if (!validationResult.IsValid)
        //{
        //    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
        //    return TypedResults.BadRequest("Failed Validation");
        //}

        var userSub = httpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userSub))
        {
            return TypedResults.BadRequest("No user found");
        }

        var entity = new CatalogItem
        {
            Id = Guid.NewGuid(),
            VendorId = vendorId,
            Name = request.Name,
            Description = request.Description,
            AddedByUserId = userSub
        }; // Todo: Mapper would be nice, right?

        session.Store(entity);
        await session.SaveChangesAsync();
        return TypedResults.Ok(entity); // Make a response model for this.
    }
}

internal record NewRecord(string Error);
