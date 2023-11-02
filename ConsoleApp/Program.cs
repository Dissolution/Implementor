
ITest test = default!;

string? str = test?.ToString();
Console.WriteLine(str);

Console.WriteLine("Press enter to quit");
Console.ReadLine();
return 0;


[Implement]
public interface ITest
{
    
}