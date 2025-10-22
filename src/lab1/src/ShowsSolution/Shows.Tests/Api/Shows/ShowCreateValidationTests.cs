
using FluentValidation.TestHelper;
using Shows.Api.Api.Shows;

namespace Shows.Tests.Api.Shows;
[Trait("Category", "Unit")]
public class ShowCreateValidationTests
{

    [Theory]
    // InlineData, MemberData, ClassData [[model, failureMessage], [model, failureMessage]
    [MemberData(nameof(ValidExamples))]
    public void ValidExamplesOfAShow(ShowCreateRequestModel model, string failureMessage)
    {
        var validator = new ShowCreateRequestValidator();
        var validations = validator.TestValidate(model);
        Assert.True(validations.IsValid, failureMessage);

    }

    [Theory]
    [MemberData(nameof(InvalidExamples))]
    public void InvalidExamplesOfAShow(ShowCreateRequestModel model, string failureMessage)
    {
        var validator = new ShowCreateRequestValidator();
        var validations = validator.TestValidate(model);
        Assert.False(validations.IsValid, failureMessage);
    }

    public static int showNameMinLength = 3;
    public static int showNameMaxLength = 100;
    public static int showDescriptionMinLength = 10;
    public static int showDescriptionMaxLength = 500;
    public static IEnumerable<object[]> ValidExamples() =>
   [
       [
            new ShowCreateRequestModel {Name = new string('X', showNameMaxLength), Description = new string('X', showDescriptionMaxLength), StreamingService="X" },
            $"The minimum length of show name should be {showNameMinLength}"
        ],
        [
            new ShowCreateRequestModel { Name = new string('X', showNameMaxLength), Description = new string('X', showDescriptionMinLength), StreamingService ="Netflix"},
            $"This should be valid, blah blah blah"
            ],
        

       ];

    public static IEnumerable<object[]> InvalidExamples() =>
        [
        [
            new ShowCreateRequestModel {Name = new string('X', showNameMinLength -1), Description=new string('X', showDescriptionMaxLength), StreamingService = "X"},
            "The Name Doesn't Match The Min Length"
            ],
        [
                new ShowCreateRequestModel { Name = new string('X', showNameMinLength), Description = new string('X', showDescriptionMinLength)},
                "Streaming Service Should Be Required"
                ]
        ];
    
}
