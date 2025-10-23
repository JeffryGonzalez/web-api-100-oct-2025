using Alba;
using Alba.Security;

namespace SoftwareCenter.Tests.CatalogItems.Fixtures;

public class CatalogItemTestFixture : IAsyncLifetime
{
    public IAlbaHost Host { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        // Use test database connection string matching appsettings.Development.json
        Host = await AlbaHost.For<Program>((config) =>
        {
            config.UseSetting("ConnectionStrings:software", "Host=localhost;port=5432;database=software;Username=postgres;password=password");
        }, new AuthenticationStub());
    }

    public async Task DisposeAsync()
    {
        await Host.DisposeAsync();
    }
}

[CollectionDefinition("CatalogItemTestFixture")]
public class CatalogItemTestFixtureCollection : ICollectionFixture<CatalogItemTestFixture>;
