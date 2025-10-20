

namespace SoftwareCenter.Tests;

[Trait("Category", "Demo")]
public class UnitTest1
{
    [Fact]
    public void CanDotnetAddTenAndTwenty()
    {
        // Given
        int a = 10; int b = 20; int answer;

        // When
        answer = a + b;

        // Then
        Assert.Equal(30, answer);
    }
    [Theory]
    [InlineData(10,20,30)]
    [InlineData(2,2, 4)]
    [InlineData(3,3,6)]
    public void CanAddAnyIntegerTogether(int a, int b, int expected)
    {

        var answer = a + b;

        Assert.Equal(expected, answer);
    }
}
