using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors.VendorManagement;

public enum ApiResults {  NotFound, Unathorized, Succceded };
public interface IManageVendors
{
    Task<VendorDetailsModel> AddVendorAsync(VendorCreateModel request);
    Task<CollectionResponseModel<VendorSummaryItem>> GetAllVendorsAsync();
    Task<VendorDetailsModel?> GetVendorByIdAsync(Guid id);
    Task<ApiResults> UpdateVendorPocAsync(Guid id, VendorPointOfContact request);
}