using Alba;
using Alba.Security;

namespace SoftwareCenter.Tests.CatalogItems;

[Trait("Category", "System")]
public class CanGetCatalogItemListBase
{
    [Fact]
    public async Task CanGetCatalogItemList()
    {
        var host = await AlbaHost.For<Program>((config) =>
        {

            //config.UseSetting("connectionstrings__software", "a test database")}, 
        },
            new AuthenticationStub());

        // here's the scenario for this test.
        await host.Scenario(api =>
        {
            api.Get.Url($"vendors/{039945930}/catalog");
            api.StatusCodeShouldBe(200);
        });
    }
}