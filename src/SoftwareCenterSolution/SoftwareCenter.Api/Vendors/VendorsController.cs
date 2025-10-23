using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors;

public class VendorsController(IManageVendors vendorManager) : ControllerBase
{

    [HttpGet("/vendors")]
    public async Task<ActionResult> GetAllVendorsAsync()
    {
       var response = await vendorManager.GetAllVendorsAsync();
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
       var response = await vendorManager.AddVendorAsync(request);      
        return StatusCode(201, response); // "Created"
    }
    [HttpGet("/vendors/{id:guid}")]
    public async Task<ActionResult> GetVendorByIdAsync(Guid id)
    {
        var response = await vendorManager.GetVendorByIdAsync(id);
        return response switch
        {
            null => NotFound(),
            _ => Ok(response)
        };
    }
}



