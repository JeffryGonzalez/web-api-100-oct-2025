

namespace DotnetAndCSharp.Zoo;
public record Monkey 
{
    public required string? Name { get; init; }
    public int Age { get; init; }
    public required string FavoriteFood { get; init; } = string.Empty;
   
}


public record Gorilla(string Name, int Age);// doing some code.