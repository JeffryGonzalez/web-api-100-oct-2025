
using Alba;
using Marten.Linq.Parsing.Operators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Shows.Api.Api.Shows;

namespace Shows.Tests.Api.Shows;
public class AddingAShowControllerTheClock
{
    [Fact]
    public async Task ShowCreatedOnMyBirthday()
    {
        var myBirthday = new DateTimeOffset(1969, 4, 20, 23, 59, 59, TimeSpan.Zero);
        FakeTimeProvider provider;
        var fakeId = Guid.Parse("e8bdad02-2563-4a02-9d01-9c5dab497e88");
        var host = await AlbaHost.For<Program>( config =>
        {
            config.ConfigureTestServices(sc =>
            {
                // I want to replace for this test one of the services on the services colletion.
                provider = new FakeTimeProvider(myBirthday);
                sc.AddSingleton<TimeProvider>(provider);
                // vefore the build this is called, and you can change thigs for your test here.
                var fakeTestIdGenerator = Substitute.For<IProvideUniqueIds>();
               
                fakeTestIdGenerator.GetGuid().Returns(fakeId);
                sc.AddSingleton<IProvideUniqueIds>(_ => fakeTestIdGenerator);

            });
        });
        var showToCreate = new ShowCreateRequestModel
        {
            Name = "Test Show",
            Description = "This is a test show",
            StreamingService = "HBO Max"
        };
        var response = await host.Scenario(_ =>
        {
            _.Post.Json(showToCreate).ToUrl("/api/shows");
            _.StatusCodeShouldBeOk();
        });

        var postBody = response.ReadAsJson<ShowDetailsResponseModel>();

        Assert.NotNull(postBody);
        Assert.Equal(fakeId, postBody.Id);
    
        Assert.Equal(postBody.CreatedAt, myBirthday);

    }
}
