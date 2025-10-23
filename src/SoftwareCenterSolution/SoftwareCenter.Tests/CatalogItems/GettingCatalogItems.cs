using System.Security.Claims;
using Alba;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Models;
using SoftwareCenter.Tests.CatalogItems.Fixtures;

namespace SoftwareCenter.Tests.CatalogItems;

[Collection("CatalogItemTestFixture")]
[Trait("Category", "System")]
public class GettingCatalogItems(CatalogItemTestFixture fixture)
{
    private readonly IAlbaHost _host = fixture.Host;

    [Fact]
    public async Task CanGetAllCatalogItemsForAVendor()
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
            api.WithClaim(new Claim(ClaimTypes.Name, "TestUser"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var vendor = vendorResponse.ReadAsJson<VendorDetailsModel>();

        // Add multiple catalog items
        var item1 = new CatalogItemCreateModel
        {
            Name = "Item One",
            Description = "First catalog item"
        };

        await _host.Scenario(api =>
        {
            api.Post.Json(item1).ToUrl($"/vendors/{vendor!.Id}/catalog");
            api.StatusCodeShouldBe(201);
        });

        var item2 = new CatalogItemCreateModel
        {
            Name = "Item Two",
            Description = "Second catalog item"
        };

        await _host.Scenario(api =>
        {
            api.Post.Json(item2).ToUrl($"/vendors/{vendor!.Id}/catalog");
            api.StatusCodeShouldBe(201);
        });

        // Get all catalog items for the vendor
        var getResponse = await _host.Scenario(api =>
        {
            api.Get.Url($"/vendors/{vendor!.Id}/catalog");
            api.StatusCodeShouldBeOk();
        });

        var catalogItems = getResponse.ReadAsJson<List<CatalogItemResponseModel>>();
        Assert.NotNull(catalogItems);
        Assert.Equal(2, catalogItems.Count);
        Assert.Contains(catalogItems, c => c.Name == "Item One");
        Assert.Contains(catalogItems, c => c.Name == "Item Two");
    }

    [Fact]
    public async Task Returns404WhenVendorDoesNotExist()
    {
        var fakeVendorId = Guid.NewGuid();

        await _host.Scenario(api =>
        {
            api.Get.Url($"/vendors/{fakeVendorId}/catalog");
            api.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task ReturnsEmptyListWhenVendorHasNoCatalogItems()
    {
        // Add vendor with no items
        var vendorToAdd = new VendorCreateModel
        {
            Name = $"Empty-Vendor-{Guid.NewGuid()}",
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
            api.WithClaim(new Claim(ClaimTypes.Name, "TestUser"));
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var vendor = vendorResponse.ReadAsJson<VendorDetailsModel>();

        // Get catalog items (should be empty)
        var getResponse = await _host.Scenario(api =>
        {
            api.Get.Url($"/vendors/{vendor!.Id}/catalog");
            api.StatusCodeShouldBeOk();
        });

        var catalogItems = getResponse.ReadAsJson<List<CatalogItemResponseModel>>();
        Assert.NotNull(catalogItems);
        Assert.Empty(catalogItems);
    }
}
