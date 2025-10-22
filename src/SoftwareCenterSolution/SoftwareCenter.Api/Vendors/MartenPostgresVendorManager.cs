using Marten;
using SoftwareCenter.Api.Vendors.Entities;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors;

public class MartenPostgresVendorManager(IDocumentSession session) : IManageVendors
{
    public async Task<VendorDetailsModel> AddVendorAsync(VendorCreateModel request)
    {
        var entity = new VendorEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PointOfContact = request.PointOfContact,
        };
        session.Store(entity);
        await session.SaveChangesAsync();

        var response = new VendorDetailsModel
        {
            Id = entity.Id,
            Name = entity.Name,
            PointOfContact = entity.PointOfContact,
        };
        return response;
    }

    public async Task<CollectionResponseModel<VendorSummaryItem>> GetAllVendorsAsync()
    {
        var vendors = await session.Query<VendorEntity>()
      .OrderBy(v => v.Name) // IQueryable<Vendor>
      .ProjectToSummary() // IQueryable<VendorSummaryItem>
      .ToListAsync();

        var response = new CollectionResponseModel<VendorSummaryItem>();
        response.Data = vendors.ToList();
       // response.Data = vendors.Select(v => v.MapFromEntity()).ToList();
        return response;
    }

    public async  Task<VendorDetailsModel?> GetVendorByIdAsync(Guid id)
    {
        var savedVendor = await session.Query<VendorEntity>()
            .Where(v => v.Id == id)
            .SingleOrDefaultAsync();
        if (savedVendor == null)
        {
            return null;
        }
        else
        {
            var response = new VendorDetailsModel
            {
                Id = savedVendor.Id,
                Name = savedVendor.Name,
                PointOfContact = savedVendor.PointOfContact,
            };
            return response;
           
        }
    }
}
