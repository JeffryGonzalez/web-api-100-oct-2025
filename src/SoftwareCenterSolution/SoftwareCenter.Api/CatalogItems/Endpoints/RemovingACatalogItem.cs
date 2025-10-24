using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;
using System.Security.Claims;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class RemovingACatalogItem
{

	public static async Task<Results<Ok, NotFound<string>, UnauthorizedHttpResult>> Handle(
		Guid vendorId,
		Guid catalogId,
		IDocumentSession session,
		IHttpContextAccessor context
	)
	{

		var vendor = session.Query<VendorEntity>()
			.Where(v => v.Id == vendorId)
			.FirstOrDefault();

		if (vendor is null)
		{
			return TypedResults.NotFound("Vendor not found.");
		}

		if (!IsUserAuthorized(context, vendor))
		{
			return TypedResults.Unauthorized();
		}

		var doesVendorAndCatalogItemExist = await session.Query<CatalogItemEntity>()
			.AnyAsync(c => c.VendorId == vendorId && c.Id == catalogId);

		if (doesVendorAndCatalogItemExist)
		{
			var response = await session.Query<CatalogItemEntity>()
				.Where(c => c.VendorId == vendorId && c.Id == catalogId)
				.FirstOrDefaultAsync();

			session.Delete<CatalogItemEntity>(response);
			await session.SaveChangesAsync();

			return TypedResults.Ok();
		} 
		else
		{
			return TypedResults.NotFound("No vendor with matching catalogue item found.");
		}
	}


	private static bool IsUserAuthorized(IHttpContextAccessor context, VendorEntity vendor)
	{
		if (context.HttpContext == null)
		{
			throw new Exception("Cannot be used in unathorized requests");
		}

		var userSub = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? throw new Exception("no sub");
		var userRoleIsSoftwareCenterManager =
			context.HttpContext.User.Claims.Contains(new Claim(ClaimTypes.Role, "SoftwareCenter"))
			&& context.HttpContext.User.Claims.Contains(new Claim(ClaimTypes.Role, "Manager"));

		return (userSub == vendor.CreatedBy) || userRoleIsSoftwareCenterManager;
	}
}
