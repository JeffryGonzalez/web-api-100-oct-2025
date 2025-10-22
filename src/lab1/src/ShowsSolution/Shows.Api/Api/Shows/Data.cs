namespace Shows.Api.Api.Shows;

public class ShowEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StreamingService { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
