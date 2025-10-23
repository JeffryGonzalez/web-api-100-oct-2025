using Microsoft.AspNetCore.Authorization;
using SoftwareCenter.Api.Vendors.Models;
using SoftwareCenter.Api.Vendors.VendorManagement;

namespace SoftwareCenter.Api.Vendors;

[Authorize]

public class VendorsController(IManageVendors vendorManager) : ControllerBase
{

    [HttpGet("/vendors")]
    public async Task<ActionResult> GetAllVendorsAsync()
    {
        var user = User.Identity;
        var response = await vendorManager.GetAllVendorsAsync();
       // return NotFound();
        return Ok(response);  
    }

    [Authorize(Policy = "software-center-manager")]
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
        var user = User.Identity;
        return response switch
        {
            null => NotFound(),
            _ => Ok(response)
        };
    }

    [Authorize(Policy = "software-center-manager")]
    [HttpPut("/vendors/{id:guid}/point-of-contact")]
    public async Task<ActionResult> UpdateVendorPointOfContact(Guid id, 
        [FromBody] VendorPointOfContact pointOfContactRequest,
        [FromServices] VendorPointOfContactValidator validator)
    {
        if (pointOfContactRequest is null)
            return BadRequest();

        var validations = await validator.ValidateAsync(pointOfContactRequest);
        if (!validations.IsValid)
        {
            return BadRequest();
        }

        var vendorCreator = await vendorManager.GetVendorCreatorAsync(id);
        if (vendorCreator is null)
            return NotFound();

        var requestor = User?.Identity?.Name ?? "";
        if (!vendorCreator.Equals(requestor))
            return StatusCode(403);

        var response = await vendorManager.UpdateVendorPointOfContactAsync(id, pointOfContactRequest);
        return response switch
        {
            null => NotFound(),
            _ => Ok(response)
        };
    }

}



