
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Shows.Api.Api.Shows;

namespace Shows.Tests.Api.Shows;
[Trait("Category", "Unit")]

public class MappingToEntityTests
{
    [Fact]
    public void CanMapModelToEntity()
    {
        var model = new ShowCreateRequestModel
        {
            Name = "Putintane",
            Description = "Some Description",
            StreamingService = "Thingy"
        };

        var fakeDate = new DateTimeOffset(1969, 4, 20, 23, 59, 59, TimeSpan.FromHours(-4));
       var fakeClock = new FakeTimeProvider(fakeDate);
        var fakeTestIdGenerator = Substitute.For<IProvideUniqueIds>();
        var fakeId = Guid.Parse("e8bdad02-2563-4a02-9d01-9c5dab497e88");
        fakeTestIdGenerator.GetGuid().Returns(fakeId);

        var result = model.MapToEntity(fakeClock, fakeTestIdGenerator);

        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.Description, result.Description);
        Assert.Equal(model.StreamingService, result.StreamingService);
        Assert.Equal(fakeDate, result.CreatedAt);
        Assert.Equal(fakeId, result.Id);
        // ?? Id?? 
    }
}

