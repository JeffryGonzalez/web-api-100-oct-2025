using Alba;
using Alba.Security;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Models;
using System.Security.Claims;

namespace SoftwareCenter.Tests.Vendors;

[Trait("Category", "System")]
public class CanRemoveACataLogItem
{

	[Fact]
	public async Task FailsToRemoveACatalogItemIfNoGuidFound()
	{

		// all authenticated users can get a list of vendors.
		// start up the api using my program.cs in memory
		var host = await AlbaHost.For<Program>((config) =>
		{

			//config.UseSetting("connectionstrings__software", "a test database")}, 
		},
			new AuthenticationStub());

		await host.Scenario(api =>
		{
			api.Delete.Url("/vendors/12341234/catalog/123141");
			api.StatusCodeShouldBe(404);
		});
	}

	[Fact]
	public async Task CanRemoveACatalogItem()
	{
		IDocumentSession? session = null;
		var host = await AlbaHost.For<Program>((config) => {

		},
			new AuthenticationStub().WithName("Violet")

			);

		// I want to check the database to make sure the name of the person that created this is saved in the database.
		// IDocumentSession is a scoped service. So I need to create a scope.
		using var scope = host.Services.CreateScope(); // dispose the scope at the end of the test.
		session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

		var vendorToAdd = new VendorCreateModel
		{
			Name = "Microsoft",
			PointOfContact = new VendorPointOfContact
			{
				Name = "Satya Nadella",
				EMail = "satya@microsoft.com",
				Phone = "800-big-corp"
			}
		};

		var postResponse = await host.Scenario(api =>
		{
			api.Post.Json(vendorToAdd).ToUrl("/vendors");
			api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
			api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
			api.StatusCodeShouldBe(201);
		});

		var postEntityReturned = postResponse.ReadAsJson<VendorDetailsModel>();


		var catalogItemToAdd = new CatalogItemCreateModel
		{
			Name = "Windows 11",
			Description = "It's not Spyware, we swear! It just kinda feels that way...",
		};

		var postCatalogItems = await host.Scenario(api =>
		{
			api.Post.Json(catalogItemToAdd).ToUrl($"/vendors/{postEntityReturned.Id}/catalog");
			//api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
			//api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
			api.StatusCodeShouldBeOk();
		});

		var postedCatalogEntityReturned = postCatalogItems.ReadAsJson<CatalogItemDetails>();
		Assert.Equal("Windows 11", postedCatalogEntityReturned.Name);
		Assert.Equal("It's not Spyware, we swear! It just kinda feels that way...", postedCatalogEntityReturned.Description);

		await host.Scenario(api =>
		{
			api.Delete.Url($"/vendors/{postEntityReturned.Id}/catalog/{postedCatalogEntityReturned.Id}");
			api.StatusCodeShouldBeOk();
		});
	}
}