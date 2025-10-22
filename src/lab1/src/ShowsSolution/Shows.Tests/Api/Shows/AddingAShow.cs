using Alba;
using Shows.Api.Api.Shows;
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

        var showToCreate = new ShowCreateRequestModel
        {
            Name = "Test Show",
            Description = "This is a test show",
            StreamingService = "HBO Max"
        };
        var response = await _host.Scenario(api =>
        {
            api.Post.Json(showToCreate).ToUrl("/api/shows");
            api.StatusCodeShouldBeOk();
        });

        var postBody = response.ReadAsJson<ShowDetailsResponseModel>();

        Assert.NotNull(postBody);
        Assert.Equal(showToCreate.Name, postBody.Name); // etc.
        Assert.Equal(showToCreate.StreamingService, postBody.StreamingService);
        
        // The Id - random Guid. // Created At
        // Math is hard. there is a difference in precision on how .NET deals with times and how Postgres does.
        //Assert.Equal(postBody.CreatedAt, DateTime.UtcNow, TimeSpan.FromMilliseconds(100));

        var getResponse = await _host.Scenario(api =>
        {
            api.Get.Url("/api/shows");
            api.StatusCodeShouldBeOk();
        });

        var getBody = getResponse.ReadAsJson<List<ShowDetailsResponseModel>>();

        Assert.NotNull(getBody);

        Assert.True(getBody.Any());

        var showAtTheTop = getBody.First();

        Assert.Equal(postBody, showAtTheTop);

    }
    
}