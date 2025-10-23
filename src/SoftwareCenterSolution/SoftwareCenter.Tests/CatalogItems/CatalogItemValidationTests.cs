using FluentValidation.TestHelper;
using SoftwareCenter.Api.CatalogItems.Models;

namespace SoftwareCenter.Tests.CatalogItems;

[Trait("Category", "Unit")]
public class CatalogItemValidationTests
{
    private readonly CatalogItemCreateModelValidator _validator = new();

    [Theory]
    [MemberData(nameof(ValidExamples))]
    public void ValidExamplesOfCatalogItem(CatalogItemCreateModel model, string failureMessage)
    {
        var validations = _validator.TestValidate(model);
        Assert.True(validations.IsValid, failureMessage);
    }

    [Theory]
    [MemberData(nameof(InvalidExamples))]
    public void InvalidExamplesOfCatalogItem(CatalogItemCreateModel model, string failureMessage)
    {
        var validations = _validator.TestValidate(model);
        Assert.False(validations.IsValid, failureMessage);
    }

    public static int NameMinLength = 3;
    public static int NameMaxLength = 100;
    public static int DescriptionMinLength = 10;
    public static int DescriptionMaxLength = 500;

    public static IEnumerable<object[]> ValidExamples() =>
    [
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength), 
                Description = new string('X', DescriptionMinLength) 
            },
            "Minimum valid lengths should be accepted"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMaxLength), 
                Description = new string('X', DescriptionMaxLength) 
            },
            "Maximum valid lengths should be accepted"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = "Visual Studio Code", 
                Description = "A great code editor for developers" 
            },
            "Normal catalog item should be valid"
        ]
    ];

    public static IEnumerable<object[]> InvalidExamples() =>
    [
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength - 1), 
                Description = new string('X', DescriptionMinLength) 
            },
            "Name shorter than minimum should fail"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMaxLength + 1), 
                Description = new string('X', DescriptionMinLength) 
            },
            "Name longer than maximum should fail"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = "", 
                Description = new string('X', DescriptionMinLength) 
            },
            "Empty name should fail"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength), 
                Description = new string('X', DescriptionMinLength - 1) 
            },
            "Description shorter than minimum should fail"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength), 
                Description = new string('X', DescriptionMaxLength + 1) 
            },
            "Description longer than maximum should fail"
        ],
        [
            new CatalogItemCreateModel 
            { 
                Name = new string('X', NameMinLength), 
                Description = "" 
            },
            "Empty description should fail"
        ]
    ];
}
