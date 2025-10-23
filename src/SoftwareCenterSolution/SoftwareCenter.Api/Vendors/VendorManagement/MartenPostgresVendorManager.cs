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
        if(context.HttpContext == null)
        {
            throw new Exception("Cannot be used in unathorized requests");
        }
        var userSub = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value  ?? throw new Exception("no sub");
        var entity = request.MapToEntity(userSub);
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

    public async Task<string?> GetVendorCreatorAsync(Guid id)
    {
        var savedVendor = await session.Query<VendorEntity>()
            .Where(v => v.Id == id)
            .SingleOrDefaultAsync();

        if (savedVendor is null)
            return default;

        return savedVendor.CreatedBy;
    }

    public async Task<VendorPointOfContact?> UpdateVendorPointOfContactAsync(Guid id, VendorPointOfContact request)
    {
        var savedVendor = await session.Query<VendorEntity>()
            .Where(v => v.Id == id)
            .SingleOrDefaultAsync();

        if (savedVendor is null)
            return default;

        savedVendor.PointOfContact = request;

        session.Store(savedVendor);
        await session.SaveChangesAsync();

        return savedVendor.PointOfContact;
    }
}
