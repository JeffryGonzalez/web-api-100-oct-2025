using FluentValidation;

namespace SoftwareCenter.Api.Vendors.Models;

/*{
  "name": "Microsoft",
  "pointOfContact": {
    "name": "Satya Nadella",
    "email": "satya@microsoft.com",
    "phone": "888 999-9999"
  }
}*/

public record VendorPointOfContact
{
    public string Name { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class VendorPointOfContactValidator : AbstractValidator<VendorPointOfContact>
{
    public VendorPointOfContactValidator()
    {
        RuleFor(v => v.Name).NotEmpty();
        RuleFor(v => v.EMail).NotEmpty().When(v => v.Phone == "");
        RuleFor(v => v.Phone).NotEmpty().When(v => v.EMail == "");
    } 
}


