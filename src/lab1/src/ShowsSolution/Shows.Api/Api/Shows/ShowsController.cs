using System.ComponentModel.DataAnnotations.Schema;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace Shows.Api.Api.Shows;

public class ShowsController(TimeProvider clock, IDocumentSession session) : ControllerBase
{
    [HttpPost("/api/shows")]
    public async Task<ActionResult> AddAShowAsync(
        [FromBody] ShowCreateRequestModel request,
        [FromServices] ShowCreateRequestValidator validator,
        [FromServices] IProvideUniqueIds uniqueIdGenerator
        )
    {

      
        var validationResults = await validator.ValidateAsync(request);

        if (!validationResults.IsValid)
        {
            return BadRequest();
        }

        // map this into the thing I'm saving in the database 

        // Map from request -> entity
        var entityToSave = request.MapToEntity(clock, uniqueIdGenerator);
   
        session.Store(entityToSave);
        await session.SaveChangesAsync();

        // Map from entity -> response 
        var response = new ShowDetailsResponseModel
        {
            Id = entityToSave.Id,
            CreatedAt = entityToSave.CreatedAt,
            Description = entityToSave.Description,
            Name = entityToSave.Name,
            StreamingService = entityToSave.StreamingService
        };
        return Ok(response);
    }

    [HttpGet("/api/shows")]
    public async Task<ActionResult> GetAllShowsAsync()
    {
        var shows = await session.Query<ShowEntity>()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(shows);
    }
}
