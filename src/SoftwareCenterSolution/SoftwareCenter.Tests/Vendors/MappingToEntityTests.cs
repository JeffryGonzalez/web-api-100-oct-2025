using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SoftwareCenter.Api.Vendors.Entities;
using SoftwareCenter.Api.Vendors.Models;
using static System.Net.WebRequestMethods;

namespace SoftwareCenter.Tests.Vendors;
[Trait("Category", "Unit")]
public class MappingToEntityTests
{
    [Fact]
    public void CanMapModelToEntity() 
    {
        var pointOfContact = new VendorPointOfContact
        { 
            Name = "ContactName",
            EMail = "contact@email.com",
            Phone = "555-555-5555"
        };
        var model = new VendorCreateModel
        {
            Name = "Name",
            PointOfContact = pointOfContact,
        };

        var entity = model.MapToEntity("Dr Doom");
        Assert.NotNull(entity);
        Assert.Equal(model.Name, entity.Name);
        Assert.Equal(model.PointOfContact, entity.PointOfContact);
        Assert.True(entity.CreatedBy == "Dr Doom");
        //Assert.Equals("Dr Doom", entity.CreatedBy);
    }

    [Fact]
    public void CanMapEntityToResponse() 
    {
        var pointOfContact = new VendorPointOfContact
        {
            Name = "ContactName",
            EMail = "contact@email.com",
            Phone = "555-555-5555"
        };
        var id = Guid.NewGuid();
        var entity = new VendorEntity
        {
            Id = id,
            Name = "Name",
            PointOfContact = pointOfContact,
        };
        var response = entity.MapToResponse();
        Assert.NotNull(response);
        Assert.Equal(entity.Name, response.Name);
        Assert.Equal(entity.PointOfContact, response.PointOfContact);
        Assert.Equal(entity.Id, response.Id);
    }

    [Fact]
    public void CanMapSummaryItemFromEntity() 
    {
        var pointOfContact = new VendorPointOfContact
        {
            Name = "ContactName",
            EMail = "contact@email.com",
            Phone = "555-555-5555"
        };
        var id = Guid.NewGuid();
        var entity = new VendorEntity
        {
            Id = id,
            Name = "Name",
        };
        var summaryItem = entity.MapFromEntity();
        Assert.NotNull(summaryItem);
        Assert.Equal(entity.Name, summaryItem.Name);
        Assert.Equal(entity.Id, summaryItem.Id);
    }
}
