


global using DotnetAndCSharp.Zoo;

Console.WriteLine("Hello, Again");

var myBowlingScores = new List<int>()
{
    127, 180, 99, 43
};

foreach(var score in myBowlingScores)
{
    Console.WriteLine(score);
}

var monkey1 = new Monkey()
{
    Name = "George"
};

var monkey2 = new Monkey()
{
    Name = "George",
    Age = 3
};
Console.WriteLine(monkey1);
Console.WriteLine(monkey2);

var kong = new Gorilla("King", 32);


//kong.Name = "King Kong";

var updatedKong = kong with { Name = "King Kong" };
Console.WriteLine(updatedKong);
Console.WriteLine(kong);

if (monkey1 == monkey2)
{
    Console.WriteLine("They are equivalent");
}
else
{
    Console.WriteLine("Not the same Monkey");
}

if (monkey1.Name != null)
{
    Console.WriteLine($"{monkey1.Name.ToUpper()}");
} else
{
    Console.WriteLine("Monkey has no name");
}