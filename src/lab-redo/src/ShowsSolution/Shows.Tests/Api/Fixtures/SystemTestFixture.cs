using Alba;

namespace Shows.Tests.Api.Fixtures;

public class SystemTestFixture : IAsyncLifetime
{
    public IAlbaHost Host { get; private set; } = null!;
    public async Task InitializeAsync()
    {
        Host = await AlbaHost.For<Program>();
    }

    public async Task DisposeAsync()
    {
        await Host.DisposeAsync();
    }
}

[CollectionDefinition("SystemTestFixture")]
public class SystemTestFixtureCollection : ICollectionFixture<SystemTestFixture>;