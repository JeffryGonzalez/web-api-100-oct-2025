using System.Security.Claims;
using Alba;
using Alba.Security;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using SoftwareCenter.Api.Vendors.Entities;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Tests.Vendors;

[Trait("Category", "System")]
public class UpdatingVendorPointOfContact
{
    [Fact]
    public async Task ManagerWhoCreatedVendorCanUpdatePointOfContact()
    {
        // Arrange
        IDocumentSession? session = null;
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        using var scope = host.Services.CreateScope();
        session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        // Create a vendor first
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Update the point of contact
        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "jane.smith@techcorp.com",
            Phone = "555-2222"
        };

        var putResponse = await host.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(200);
        });

        // Assert
        var updatedVendor = putResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(updatedVendor);
        Assert.Equal("Jane Smith", updatedVendor.PointOfContact.Name);
        Assert.Equal("jane.smith@techcorp.com", updatedVendor.PointOfContact.EMail);
        Assert.Equal("555-2222", updatedVendor.PointOfContact.Phone);

        // Verify in database
        var savedEntity = await session.Query<VendorEntity>().SingleAsync(v => v.Id == createdVendor.Id);
        Assert.Equal("Jane Smith", savedEntity.PointOfContact.Name);
        Assert.Equal("jane.smith@techcorp.com", savedEntity.PointOfContact.EMail);
        Assert.Equal("555-2222", savedEntity.PointOfContact.Phone);
    }

    [Fact]
    public async Task ManagerWhoDidNotCreateVendorCannotUpdatePointOfContact()
    {
        // Arrange
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        // Create a vendor as Alice
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Try to update as Bob (different manager)
        var hostAsBob = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Bob"));

        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "jane.smith@techcorp.com",
            Phone = "555-2222"
        };

        // Assert - Should get 403 Forbidden
        await hostAsBob.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(403);
        });
    }

    [Fact]
    public async Task NonManagersCannotUpdatePointOfContact()
    {
        // Arrange
        var hostAsManager = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        // Create a vendor as Alice (manager)
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await hostAsManager.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Try to update as regular employee (not manager)
        var hostAsEmployee = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "jane.smith@techcorp.com",
            Phone = "555-2222"
        };

        // Assert - Should get 403 Forbidden (no manager role)
        await hostAsEmployee.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.StatusCodeShouldBe(403);
        });
    }

    [Fact]
    public async Task UnauthenticatedUsersCannotUpdatePointOfContact()
    {
        // Arrange
        var host = await AlbaHost.For<Program>();

        var vendorId = Guid.NewGuid();
        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "jane.smith@techcorp.com",
            Phone = "555-2222"
        };

        // Act & Assert - Should get 401 Unauthorized
        await host.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{vendorId}/point-of-contact");
            api.StatusCodeShouldBe(401);
        });
    }

    [Fact]
    public async Task CannotUpdatePointOfContactForNonExistentVendor()
    {
        // Arrange
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        var nonExistentVendorId = Guid.NewGuid();
        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "jane.smith@techcorp.com",
            Phone = "555-2222"
        };

        // Act & Assert - Should get 404 Not Found
        await host.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{nonExistentVendorId}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(404);
        });
    }

    [Fact]
    public async Task ValidationFailsWhenNameIsEmpty()
    {
        // Arrange
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        // Create a vendor first
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Try to update with empty name
        var invalidPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "",
            EMail = "jane.smith@techcorp.com",
            Phone = "555-2222"
        };

        // Assert - Should get 400 Bad Request
        await host.Scenario(api =>
        {
            api.Put.Json(invalidPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task ValidationFailsWhenBothEmailAndPhoneAreEmpty()
    {
        // Arrange
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        // Create a vendor first
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Try to update with both email and phone empty
        var invalidPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "",
            Phone = ""
        };

        // Assert - Should get 400 Bad Request
        await host.Scenario(api =>
        {
            api.Put.Json(invalidPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(400);
        });
    }

    [Fact]
    public async Task CanUpdateWithOnlyEmailProvided()
    {
        // Arrange
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        // Create a vendor first
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Update with only email (no phone)
        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "jane.smith@techcorp.com",
            Phone = ""
        };

        var putResponse = await host.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(200);
        });

        // Assert
        var updatedVendor = putResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(updatedVendor);
        Assert.Equal("Jane Smith", updatedVendor.PointOfContact.Name);
        Assert.Equal("jane.smith@techcorp.com", updatedVendor.PointOfContact.EMail);
    }

    [Fact]
    public async Task CanUpdateWithOnlyPhoneProvided()
    {
        // Arrange
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Alice"));

        // Create a vendor first
        var vendorToAdd = new VendorCreateModel
        {
            Name = "TechCorp",
            PointOfContact = new VendorPointOfContact
            {
                Name = "John Doe",
                EMail = "john@techcorp.com",
                Phone = "555-1111"
            }
        };

        var postResponse = await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(201);
        });

        var createdVendor = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(createdVendor);

        // Act - Update with only phone (no email)
        var updatedPointOfContact = new VendorPointOfContactUpdateModel
        {
            Name = "Jane Smith",
            EMail = "",
            Phone = "555-2222"
        };

        var putResponse = await host.Scenario(api =>
        {
            api.Put.Json(updatedPointOfContact).ToUrl($"/vendors/{createdVendor.Id}/point-of-contact");
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.StatusCodeShouldBe(200);
        });

        // Assert
        var updatedVendor = putResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(updatedVendor);
        Assert.Equal("Jane Smith", updatedVendor.PointOfContact.Name);
        Assert.Equal("555-2222", updatedVendor.PointOfContact.Phone);
    }
}
