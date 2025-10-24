using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.CatalogItems.Endpoints;

public static class RemovingACatalogItem
{

	public static async Task<Results<Ok, NotFound<string>>> Handle(
		Guid vendorId,
		Guid catalogId,
		IDocumentSession session
	)
	{

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
}
