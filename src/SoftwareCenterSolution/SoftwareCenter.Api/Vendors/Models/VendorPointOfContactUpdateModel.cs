using FluentValidation;

namespace SoftwareCenter.Api.Vendors.Models;

public record VendorPointOfContactUpdateModel
{
    public string Name { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class VendorPointOfContactUpdateModelValidator : AbstractValidator<VendorPointOfContactUpdateModel>
{
    public VendorPointOfContactUpdateModelValidator()
    {
        RuleFor(v => v.Name).NotEmpty();
        RuleFor(v => v.EMail).NotEmpty().When(v => v.Phone == "");
        RuleFor(v => v.Phone).NotEmpty().When(v => v.EMail == "");
    }
}
