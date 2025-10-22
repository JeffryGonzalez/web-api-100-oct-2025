using FluentValidation;
using Marten;

namespace SoftwareCenter.Api.Vendors.Models;

// this represents what we are expecting from the client on the POST /vendors
public record VendorCreateModel
{
   
    public string Name { get; set; } = string.Empty;
    public VendorPointOfContact PointOfContact { get; set; } = new();
}


public class VendorCreateModelValidator : AbstractValidator<VendorCreateModel>
{
    public VendorCreateModelValidator()
    {
        
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.PointOfContact).NotNull().SetValidator(validator: new VendorPointOfContactValidator());
    }
}
