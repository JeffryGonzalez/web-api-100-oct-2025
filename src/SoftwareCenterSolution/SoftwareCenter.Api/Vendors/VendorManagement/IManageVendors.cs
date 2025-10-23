using SoftwareCenter.Api.Common;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors.VendorManagement;


public interface IManageVendors
{
    Task<VendorDetailsModel> AddVendorAsync(VendorCreateModel request);
    Task<CollectionResponseModel<VendorSummaryItem>> GetAllVendorsAsync();
    Task<VendorDetailsModel?> GetVendorByIdAsync(Guid id);
    Task<ApiResults> UpdateVendorPocAsync(Guid id, VendorPointOfContact request);
}