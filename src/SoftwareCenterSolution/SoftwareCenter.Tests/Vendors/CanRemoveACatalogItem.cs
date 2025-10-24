using Alba;
using Alba.Security;

namespace SoftwareCenter.Tests.Vendors;

[Trait("Category", "System")]
public class CanRemoveACataLogItem
{
	[Fact]
	public async Task CanRemoveACatalogItem()
	{

		// all authenticated users can get a list of vendors.
		// start up the api using my program.cs in memory
		var host = await AlbaHost.For<Program>((config) =>
		{

			//config.UseSetting("connectionstrings__software", "a test database")}, 
		},
			new AuthenticationStub());

		// here's the scenario for this test.
		await host.Scenario(api =>
		{
			// get the vendors (no host or anything, it's internal)
			api.Get.Url("/vendors");
			// if it isn't this, fail.
			api.StatusCodeShouldBe(200);
		});

	}
}