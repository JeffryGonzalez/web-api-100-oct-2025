using Alba;
using Shows.Tests.Api.Fixtures;

namespace Shows.Tests.Api.Shows;

[Collection("SystemTestFixture")]
[Trait("Category", "SystemTest")]
public class AddingAShow(SystemTestFixture fixture)
{
    private readonly IAlbaHost _host = fixture.Host;

    [Fact]
    public async Task AddShow()
    {
        var response = await _host.Scenario(_ =>
        {
            _.Post.Json(new
            {
                Name = "Test Show",
                Description = "This is a test show",
                StreamingService = "HBO Max"
            }).ToUrl("/api/shows");
            _.StatusCodeShouldBeOk();
        });
        

    }
    
}