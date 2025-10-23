using Microsoft.AspNetCore.Authorization;
using SoftwareCenter.Api.Vendors.Models;
using SoftwareCenter.Api.Vendors.VendorManagement;
using System.Security.Claims;

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
    [HttpPut("/vendors/{vendorId:guid}/point-of-contact")]
    public async Task<ActionResult> UpdatePointOfContactAsync(
        Guid vendorId,
        [FromBody] VendorPointOfContactUpdateModel request,
        [FromServices] VendorPointOfContactUpdateModelValidator validator)
    {
        var validations = await validator.ValidateAsync(request);
        if (!validations.IsValid)
        {
            return BadRequest(validations.Errors);
        }

        var userSub = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(userSub))
        {
            return Unauthorized();
        }

        try
        {
            var response = await vendorManager.UpdatePointOfContactAsync(vendorId, request, userSub);
            return response switch
            {
                null => NotFound(),
                _ => Ok(response)
            };
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, new { message = "Only the manager who created this vendor can update the point of contact" });
        }
    }
}



