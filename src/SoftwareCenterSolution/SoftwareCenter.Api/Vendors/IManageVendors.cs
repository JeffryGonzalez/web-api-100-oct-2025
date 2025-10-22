using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors;

public interface IManageVendors
{
    Task<VendorDetailsModel> AddVendorAsync(VendorCreateModel request);
    Task<CollectionResponseModel<VendorSummaryItem>> GetAllVendorsAsync();
    Task<VendorDetailsModel?> GetVendorByIdAsync(Guid id);
}