using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors;

public static class VendorExtensions
{
    public static IServiceCollection AddVendorServices(this IServiceCollection services)
    {
        services.AddScoped<VendorCreateModelValidator>();
        services.AddScoped<VendorPointOfContactUpdateModelValidator>();
        return services;
    }
}
