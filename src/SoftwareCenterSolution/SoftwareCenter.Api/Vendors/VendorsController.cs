using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors;


// When we get a GET request to "/vendors", we want this controller to be created, and
// a specific method on this controller to handle providing the response for the request.

public class VendorsController(IManageVendors vendorManager) : ControllerBase
{
    // controllers should receive the http request, validate it,
    // and create a response to send to the caller.



    [HttpGet("/vendors")]
    public async Task<ActionResult> GetAllVendorsAsync()
    {

        CollectionResponseModel<VendorSummaryItem> response = await vendorManager.GetAllVendorsAsync();
        return Ok(response);
       
    }

    [HttpPost("/vendors")]
    public async Task<ActionResult> AddVendorAsync(
        [FromBody] VendorCreateModel request,
        [FromServices] VendorCreateModelValidator validator
        )

    {

       var validations = await validator.ValidateAsync(request);

        if(!validations.IsValid)
        {
            return BadRequest();
        }

     

        VendorDetailsModel response = await vendorManager.AddVendorAsync(request);

        
        return StatusCode(201, response); // "Created"
    }
    // GET /vendors/tacos
    [HttpGet("/vendors/{id:guid}")]
    public async Task<ActionResult> GetVendorByIdAsync(Guid id)
    {
        VendorDetailsModel? response = await vendorManager.GetVendorByIdAsync(id);
        return response switch
        {
            null => NotFound(),
            _ => Ok(response)
        };
    }
}



