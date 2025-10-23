using Alba;
using SoftwareCenter.Api.CatalogItems.Models;
using SoftwareCenter.Tests.CatalogItems.Fixtures;

namespace SoftwareCenter.Tests.CatalogItems;

[Collection("CatalogItemTestFixture")]
[Trait("Category", "System")]
public class CatalogItemsAreValidatedWhenCreated(CatalogItemTestFixture fixture)
{
    private readonly IAlbaHost _host = fixture.Host;

    [Theory]
    [MemberData(nameof(InvalidExamples))]
    public async Task BadRequestsReturnA400(CatalogItemCreateModel model, string _)
    {
        var vendorId = Guid.NewGuid(); // doesn't matter for validation

        await _host.Scenario(api =>
        {
            api.Post.Json(model).ToUrl($"/vendors/{vendorId}/catalog");
            api.StatusCodeShouldBe(400);
        });
    }

    public static int NameMinLength = 3;
    public static int NameMaxLength = 100;
    public static int DescriptionMinLength = 10;
    public static int DescriptionMaxLength = 500;

    public static IEnumerable<object[]> InvalidExamples() =>
    [
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength - 1), 
                Description = new string('X', DescriptionMinLength) 
            },
            "Name shorter than minimum should return 400"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMaxLength + 1), 
                Description = new string('X', DescriptionMinLength) 
            },
            "Name longer than maximum should return 400"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = "", 
                Description = new string('X', DescriptionMinLength) 
            },
            "Empty name should return 400"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength), 
                Description = new string('X', DescriptionMinLength - 1) 
            },
            "Description shorter than minimum should return 400"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength), 
                Description = "" 
            },
            "Empty description should return 400"
        ]
    ];
}
