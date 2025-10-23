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
public class AddingCatalogItems(CatalogItemTestFixture fixture)
{
    private readonly IAlbaHost _host = fixture.Host;

    [Fact]
    public async Task CanAddACatalogItemToAVendor()
    {
        // First, add a vendor
        var vendorToAdd = new VendorCreateModel
        {
            Name = "Microsoft",
            PointOfContact = new VendorPointOfContact
            {
                Name = "Satya Nadella",
                EMail = "satya@microsoft.com",
                Phone = "800-microsoft"
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
        Assert.NotNull(vendor);

        // Now add a catalog item
        var catalogItemToAdd = new CatalogItemCreateModel
        {
            Name = "Visual Studio 2025",
            Description = "The latest version of Visual Studio with amazing features"
        };

        var catalogResponse = await _host.Scenario(api =>
        {
            api.Post.Json(catalogItemToAdd).ToUrl($"/vendors/{vendor.Id}/catalog");
            api.WithClaim(new Claim(ClaimTypes.Name, "TestUser"));
            api.StatusCodeShouldBe(201);
        });

        var catalogItem = catalogResponse.ReadAsJson<CatalogItemResponseModel>();
        Assert.NotNull(catalogItem);
        Assert.NotEqual(Guid.Empty, catalogItem.Id);
        Assert.Equal(vendor.Id, catalogItem.VendorId);
        Assert.Equal(catalogItemToAdd.Name, catalogItem.Name);
        Assert.Equal(catalogItemToAdd.Description, catalogItem.Description);

        // Verify it was saved with CreatedBy
        using var scope = _host.Services.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
        var savedItem = await session.Query<CatalogItem>().SingleAsync(c => c.Id == catalogItem.Id);
        Assert.Equal("TestUser", savedItem.CreatedBy);
    }

    [Fact]
    public async Task CannotAddDuplicateCatalogItemNameForSameVendor()
    {
        // First, add a vendor
        var vendorToAdd = new VendorCreateModel
        {
            Name = "Adobe",
            PointOfContact = new VendorPointOfContact
            {
                Name = "Adobe Contact",
                EMail = "contact@adobe.com",
                Phone = "800-adobe"
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
        Assert.NotNull(vendor);

        // Add first catalog item
        var catalogItemToAdd = new CatalogItemCreateModel
        {
            Name = "Photoshop",
            Description = "Photo editing software for professionals"
        };

        await _host.Scenario(api =>
        {
            api.Post.Json(catalogItemToAdd).ToUrl($"/vendors/{vendor.Id}/catalog");
            api.StatusCodeShouldBe(201);
        });

        // Try to add duplicate - should fail with 409 Conflict
        await _host.Scenario(api =>
        {
            api.Post.Json(catalogItemToAdd).ToUrl($"/vendors/{vendor.Id}/catalog");
            api.StatusCodeShouldBe(409);
        });
    }

    [Fact]
    public async Task DifferentVendorsCanHaveSameItemName()
    {
        // Add two vendors
        var vendor1 = new VendorCreateModel
        {
            Name = "Vendor One",
            PointOfContact = new VendorPointOfContact
            {
                Name = "Contact One",
                EMail = "one@vendor.com",
                Phone = "111-1111"
            }
        };

        var vendor1Response = await _host.Scenario(api =>
        {
            api.Post.Json(vendor1).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Name, "TestUser"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var vendor1Data = vendor1Response.ReadAsJson<VendorDetailsModel>();

        var vendor2 = new VendorCreateModel
        {
            Name = "Vendor Two",
            PointOfContact = new VendorPointOfContact
            {
                Name = "Contact Two",
                EMail = "two@vendor.com",
                Phone = "222-2222"
            }
        };

        var vendor2Response = await _host.Scenario(api =>
        {
            api.Post.Json(vendor2).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Name, "TestUser"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var vendor2Data = vendor2Response.ReadAsJson<VendorDetailsModel>();

        // Same item name for both vendors
        var catalogItem = new CatalogItemCreateModel
        {
            Name = "Enterprise Suite",
            Description = "A comprehensive enterprise software solution"
        };

        // Add to vendor 1
        await _host.Scenario(api =>
        {
            api.Post.Json(catalogItem).ToUrl($"/vendors/{vendor1Data!.Id}/catalog");
            api.StatusCodeShouldBe(201);
        });

        // Add same name to vendor 2 - should succeed
        await _host.Scenario(api =>
        {
            api.Post.Json(catalogItem).ToUrl($"/vendors/{vendor2Data!.Id}/catalog");
            api.StatusCodeShouldBe(201);
        });
    }

    [Fact]
    public async Task CannotAddCatalogItemToNonExistentVendor()
    {
        var catalogItem = new CatalogItemCreateModel
        {
            Name = "Test Item",
            Description = "This should fail because vendor doesn't exist"
        };

        var fakeVendorId = Guid.NewGuid();

        await _host.Scenario(api =>
        {
            api.Post.Json(catalogItem).ToUrl($"/vendors/{fakeVendorId}/catalog");
            api.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task UnauthenticatedUserCannotAddCatalogItem()
    {
        var catalogItem = new CatalogItemCreateModel
        {
            Name = "Test Item",
            Description = "This should fail due to no authentication"
        };

        var host = await AlbaHost.For<Program>();

        await host.Scenario(api =>
        {
            api.Post.Json(catalogItem).ToUrl($"/vendors/{Guid.NewGuid()}/catalog");
            api.StatusCodeShouldBe(401);
        });
    }
}
