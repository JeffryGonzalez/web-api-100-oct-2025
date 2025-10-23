using Microsoft.CodeAnalysis.CSharp.Syntax;
using Riok.Mapperly.Abstractions;
using SoftwareCenter.Api.CatalogItems.Entities;

namespace SoftwareCenter.Api.CatalogItems.Models;

[Mapper]
public  static partial class Mappers
{
    public static partial IQueryable<CatalogItemDetails> ProjectTo(this IQueryable<CatalogItemEntity> entities);
   
    public  static partial CatalogItemDetails MapToDetails(this CatalogItemEntity entity);
}
