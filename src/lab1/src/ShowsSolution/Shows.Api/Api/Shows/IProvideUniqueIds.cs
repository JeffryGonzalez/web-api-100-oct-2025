namespace Shows.Api.Api.Shows;

public interface IProvideUniqueIds
{
    Guid GetGuid();
}

public class SystemUniqueIdProvider : IProvideUniqueIds
{
    public Guid GetGuid()
    {
        return Guid.NewGuid();
    }
}
