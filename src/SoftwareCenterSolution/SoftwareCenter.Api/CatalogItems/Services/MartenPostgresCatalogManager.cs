using Marten;
using SoftwareCenter.Api.CatalogItems.Entities;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Api.Vendors.Entities;

namespace SoftwareCenter.Api.CatalogItems.Services;

public class MartenPostgresCatalogManager(IDocumentSession session)
{
    public async Task< (ApiResults, CatalogItemDetails?)> AddCatalogItemAsync(CatalogItemCreateModel model, Guid vendorId)
    {
        var vendorName = await session.Query<VendorEntity>()
              .Where(v => v.Id == vendorId)
              .Select(v => v.Name)
              .SingleOrDefaultAsync();
        if (vendorName == null)
        {
           return  (ApiResults.NotFound, null);
        }
        var entity = new CatalogItemEntity
        {
            Id = Guid.NewGuid(),
            VendorId = vendorId,
            VendorName = vendorName,
            Name = model.Name,
            Description = model.Description,
        }; // Todo: Mapper would be nice, right?

        session.Store(entity);
        await session.SaveChangesAsync();
        return (ApiResults.Succceded, entity.MapToDetails());
    }
}
