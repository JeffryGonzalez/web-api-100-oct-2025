using System.Security.Claims;
using Alba;
using Alba.Security;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;
using SoftwareCenter.Api.Vendors.Models;
using SoftwareCenter.Tests.CatalogItems.Fixtures;

namespace SoftwareCenter.Tests.CatalogItems;

[Collection("CatalogItemTestFixture")]
[Trait("Category", "System")]
public class RemovingCatalogItems(CatalogItemTestFixture fixture)
{
    private readonly IAlbaHost _host = fixture.Host;

    [Fact]
    public async Task CreatorCanRemoveTheirOwnCatalogItem()
    {
        // Setup: Add vendor and catalog item
        var (vendorId, catalogItemId) = await SetupVendorAndCatalogItem("Creator1");

        // Creator deletes their own item
        await _host.Scenario(api =>
        {
            api.Delete.Url($"/vendors/{vendorId}/catalog/{catalogItemId}");
            api.WithClaim(new Claim(ClaimTypes.Name, "Creator1"));
            api.StatusCodeShouldBe(204);
        });

        // Verify it's deleted
        using var scope = _host.Services.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
        var deleted = await session.Query<CatalogItem>().AnyAsync(c => c.Id == catalogItemId);
        Assert.False(deleted);
    }

    [Fact]
    public async Task ManagerCanRemoveAnyCatalogItem()
    {
        // Setup: Add vendor and catalog item created by someone else
        var (vendorId, catalogItemId) = await SetupVendorAndCatalogItem("Creator2");

        // Manager deletes someone else's item
        await _host.Scenario(api =>
        {
            api.Delete.Url($"/vendors/{vendorId}/catalog/{catalogItemId}");
            api.WithClaim(new Claim(ClaimTypes.Name, "ManagerUser"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(204);
        });
    }

    [Fact]
    public async Task NonCreatorNonManagerCannotRemoveCatalogItem()
    {
        // Setup: Add vendor and catalog item
        var (vendorId, catalogItemId) = await SetupVendorAndCatalogItem("Creator3");

        // Different user (not creator, not manager) tries to delete
        await _host.Scenario(api =>
        {
            api.Delete.Url($"/vendors/{vendorId}/catalog/{catalogItemId}");
            api.WithClaim(new Claim(ClaimTypes.Name, "DifferentUser"));
            api.StatusCodeShouldBe(403);
        });
    }

    [Fact]
    public async Task UnauthenticatedUserCannotRemoveCatalogItem()
    {
        var host = await AlbaHost.For<Program>();

        await host.Scenario(api =>
        {
            api.Delete.Url($"/vendors/{Guid.NewGuid()}/catalog/{Guid.NewGuid()}");
            api.StatusCodeShouldBe(401);
        });
    }

    [Fact]
    public async Task Returns404WhenCatalogItemDoesNotExist()
    {
        // Add a vendor but no catalog item
        var vendorToAdd = new VendorCreateModel
        {
            Name = "Test Vendor",
            PointOfContact = new VendorPointOfContact
            {
                Name = "Contact",
                EMail = "test@test.com",
                Phone = "123-4567"
            }
        };

        var vendorResponse = await _host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Name, "TestUser"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var vendor = vendorResponse.ReadAsJson<VendorDetailsModel>();

        // Try to delete non-existent catalog item
        await _host.Scenario(api =>
        {
            api.Delete.Url($"/vendors/{vendor!.Id}/catalog/{Guid.NewGuid()}");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(404);
        });
    }

    private async Task<(Guid vendorId, Guid catalogItemId)> SetupVendorAndCatalogItem(string creatorName)
    {
        // Add vendor
        var vendorToAdd = new VendorCreateModel
        {
            Name = $"Vendor-{Guid.NewGuid()}",
            PointOfContact = new VendorPointOfContact
            {
                Name = "Contact",
                EMail = "contact@vendor.com",
                Phone = "800-vendor"
            }
        };

        var vendorResponse = await _host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Name, "VendorCreator"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var vendor = vendorResponse.ReadAsJson<VendorDetailsModel>();

        // Add catalog item
        var catalogItemToAdd = new CatalogItemCreateModel
        {
            Name = $"Item-{Guid.NewGuid()}",
            Description = "Test catalog item for deletion tests"
        };

        var catalogResponse = await _host.Scenario(api =>
        {
            api.Post.Json(catalogItemToAdd).ToUrl($"/vendors/{vendor!.Id}/catalog");
            api.WithClaim(new Claim(ClaimTypes.Name, creatorName));
            api.StatusCodeShouldBe(201);
        });

        var catalogItem = catalogResponse.ReadAsJson<CatalogItemResponseModel>();

        return (vendor!.Id, catalogItem!.Id);
    }
}
