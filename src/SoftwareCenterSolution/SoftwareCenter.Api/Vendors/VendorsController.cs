

namespace SoftwareCenter.Api.Vendors;


// When we get a GET request to "/vendors", we want this controller to be created, and
// a specific method on this controller to handle providing the response for the request.

public class VendorsController : ControllerBase
{
    [HttpGet("/vendors")]
    public async Task<ActionResult> GetAllVendorsAsync()
    {
        return Ok();
    }
}
