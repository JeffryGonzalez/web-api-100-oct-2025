using System.Security.Claims;
using ImTools;
using JasperFx.CodeGeneration.Frames;
using Marten;
using SoftwareCenter.Api.Vendors.Entities;
using SoftwareCenter.Api.Vendors.Models;

namespace SoftwareCenter.Api.Vendors.VendorManagement;

public class MartenPostgresVendorManager(IDocumentSession session, IHttpContextAccessor context) : IManageVendors
{
    public async Task<VendorDetailsModel> AddVendorAsync(VendorCreateModel request)
    {
        var entity = request.MapToEntity(GetUserSub());
        session.Store(entity);
        await session.SaveChangesAsync();
        return entity.MapToResponse();
    }

    public async Task<CollectionResponseModel<VendorSummaryItem>> GetAllVendorsAsync()
    {
        var vendors = await session.Query<VendorEntity>()
          .OrderBy(v => v.Name) // IQueryable<Vendor>
          .ProjectToSummary() // IQueryable<VendorSummaryItem>
          .ToListAsync();

        var response = new CollectionResponseModel<VendorSummaryItem>();
        response.Data = [.. vendors];
        return response;
    }

    public async Task<VendorDetailsModel?> GetVendorByIdAsync(Guid id)
    {
        var savedVendor = await session.Query<VendorEntity>()
            .Where(v => v.Id == id)
            .SingleOrDefaultAsync();
        return savedVendor switch
        {
            null => null,
            _ => savedVendor.MapToResponse()
        };

    }

    public async Task<ApiResults> UpdateVendorPocAsync(Guid id, VendorPointOfContact request)
    {
        var vendor = await session.Query<VendorEntity>().SingleOrDefaultAsync(v => v.Id == id);
        if (vendor == null)
        {
            return ApiResults.NotFound;
        }
        if(vendor.CreatedBy != GetUserSub())
        {
            return ApiResults.Unathorized;
        }
        vendor.PointOfContact = request;
        session.Store(vendor);


        await session.SaveChangesAsync();
        return ApiResults.Succceded;
    }

    private string GetUserSub()
    {
        if (context.HttpContext == null)
        {
            throw new Exception("Cannot be used in unathorized requests");
        }
        var userSub = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? throw new Exception("no sub");
        return userSub;
    }
}
