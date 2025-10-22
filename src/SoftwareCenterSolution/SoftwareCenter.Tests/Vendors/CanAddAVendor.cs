
using Alba;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Tests.Vendors;
[Trait("Category", "System")]

public class CanAddAVendor
{

    [Fact]
    public async Task AddingAVendor()
    {
        var host = await AlbaHost.For<Program>();

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
   // adding a vendor returns a success status code (probably a 201)

    // adding a vendor saves it to the database, and we can verify that.

    // only managers of the software center can do this.

    // employees that aren't managers get a 403

    // non-authenticated users get a 401

}
