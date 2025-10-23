
using System.Security.Claims;
using Alba;
using Alba.Security;
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

            config.ConfigureServices(sp =>
            {
                    // you can get the IDocumentsession here and store it in 
                    // a variable and do what kyle said - make sure this wasn't added.

            });
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
        var host = await AlbaHost.For<Program>((config) => { },
            new AuthenticationStub().WithName("Violet")
    
            );

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
