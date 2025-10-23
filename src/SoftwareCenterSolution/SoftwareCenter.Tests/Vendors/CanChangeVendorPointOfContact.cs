

using System.Security.Claims;
using Alba;
using Alba.Security;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Tests.Vendors;
[Trait("Category", "System")]
public class CanChangeVendorPointOfContact
{
    [Fact]
    public async Task CanChangeAVendorsPointOfContact()
    {
        // Given a manager has created a vendor
        // Manager is "George"
        // That same manager can change the point of contact.
        var host = await AlbaHost.For<Program>(config => { }, new AuthenticationStub().WithName("George"));

        var originalPoc = new VendorPointOfContact
        {
            Name = "Carl",
            EMail = "carl@company.com"
        };
        var updatedPoc = new VendorPointOfContact
        {
            Name = "Brianna",
            Phone = "800 999-9999"
        };
        var vendorToCreate = new VendorCreateModel
        {
            Name = "Jetbrains",
            PointOfContact = originalPoc
        };

        var postResponse = await host.Scenario(api =>
        {
           
            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.Post.Json(vendorToCreate).ToUrl("/vendors");
            api.StatusCodeShouldBe(201);
        });

        var postResponseBody = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(postResponseBody);
        Assert.Equal(originalPoc, postResponseBody.PointOfContact);

        var url = $"/vendors/{postResponseBody.Id}";

        await host.Scenario(api =>
        {
            api.Put.Json(updatedPoc).ToUrl($"{url}/point-of-contact");
            api.StatusCodeShouldBe(202);
        });

        var getResponse = await host.Scenario(api =>
        {
            api.Get.Url(url);

        });

        var getBodyResponse = getResponse.ReadAsJson<VendorDetailsModel>();

        Assert.NotNull(getBodyResponse);

        Assert.Equal(updatedPoc, getBodyResponse.PointOfContact);
    }

    [Fact()]
    public async Task CannotChangeAVendorYouDidNotCreate()
    {
        var briannaHost = await AlbaHost.For<Program>(config => { }, new AuthenticationStub().WithName("George"));

        var originalPoc = new VendorPointOfContact
        {
            Name = "Carl",
            EMail = "carl@company.com"
        };
        var updatedPoc = new VendorPointOfContact
        {
            Name = "Brianna",
            Phone = "800 999-9999"
        };
        var vendorToCreate = new VendorCreateModel
        {
            Name = "Jetbrains",
            PointOfContact = originalPoc
        };

        var postResponse = await briannaHost.Scenario(api =>
        {

            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.Post.Json(vendorToCreate).ToUrl("/vendors");
            api.StatusCodeShouldBe(201);
        });

        var postResponseBody = postResponse.ReadAsJson<VendorDetailsModel>();
        Assert.NotNull(postResponseBody);

        var url = $"/vendors/{postResponseBody.Id}";



        var otherManagerHost = await AlbaHost.For<Program>(config => { }, new AuthenticationStub().WithName("Paul"));

        await otherManagerHost.Scenario(api =>
        {

            api.WithClaim(new Claim(ClaimTypes.Role, "SoftwareCenter"));
            api.WithClaim(new Claim(ClaimTypes.Role, "Manager"));
            api.Put.Json(updatedPoc).ToUrl($"{url}/point-of-contact");
            api.StatusCodeShouldBe(403);
        });
    }

    [Fact(Skip = "Needed - Check to make sure of the authn/authz")]
    public void AuthCheck()
    {

    }
}
