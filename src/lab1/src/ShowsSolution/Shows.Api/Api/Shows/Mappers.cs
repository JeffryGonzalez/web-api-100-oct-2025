using System.Xml.Linq;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Shows.Api.Api.Shows;



// Map from request -> entity
public static class Mappers
{
    public static ShowEntity MapToEntity(this ShowCreateRequestModel model, TimeProvider clock, IProvideUniqueIds idGenerator)
    {
        return new ShowEntity
        {
            Id = idGenerator.GetGuid(),
            CreatedAt = clock.GetUtcNow(),
            Description = model.Description,
            Name = model.Name,
            StreamingService = model.StreamingService,
        };
    }
}