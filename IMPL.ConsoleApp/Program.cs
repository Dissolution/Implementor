using IMPL.Contracts;




Console.WriteLine("Press Enter to close.");
Console.ReadLine();


[Implement]
public interface ITestInterface : IEquatable<ITestInterface>
{
    int Id { get; init; }
}