
using System.Security.Claims;
using Alba;
using Alba.Security;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using SoftwareCenter.Api.Vendors.Entities;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Tests.Vendors;
[Trait("Category", "System")]

public class CanAddAVendor
{

    [Fact]
    public async Task NonManagersCannotAddAVendor()
    {
        // You are authenticated, but NOT in the SoftwareCenter or Manager roles.
        var host = await AlbaHost.For<Program>((config) => {
            
        },
            new AuthenticationStub());

     
        
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

       await host.Scenario(api =>
        {
            api.Post.Json(vendorToAdd).ToUrl("/vendors");
            api.StatusCodeShouldBe(403);
        });

     
    }

    [Fact]
    public async Task ManagersCanAddAVendor()
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

        Assert.NotNull(postEntityReturned);

        Assert.True(postEntityReturned.Id != Guid.Empty);
        Assert.Equal(postEntityReturned.Name, vendorToAdd.Name);
        Assert.Equal(postEntityReturned.PointOfContact, vendorToAdd.PointOfContact);

        // Query the database to check the CreatedBy=="Violet"
        var savedEntity = await session.Query<VendorEntity>().SingleAsync(v => v.Id == postEntityReturned.Id);

        Assert.Equal("Violet", savedEntity.CreatedBy);


        var getResponse = await host.Scenario(api =>
        {
            api.Get.Url($"/vendors/{postEntityReturned.Id}");
            api.StatusCodeShouldBeOk();

        });

        var getEntityReturned = getResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(getEntityReturned);
        Assert.Equal(postEntityReturned, getEntityReturned);
    }

    // only managers of the software center can do this.

    // employees that aren't managers get a 403

    // non-authenticated users get a 401

}
