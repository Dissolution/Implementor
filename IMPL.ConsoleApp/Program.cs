using IMPL.Contracts;

//TestInterface test = new TestInterface();

//var type = test.GetType();
//Console.WriteLine(type);

Console.WriteLine("Press Enter to close.");
Console.ReadLine();


[Implement(Name = "Tester", Declaration = "internal")]
public interface ITestInterface : IDisposable
{
    int Id { get; }
    string? Name { get; set; }

    event EventHandler Exploded;
}

//[Implement]
//public partial class TestClass
//{

//}
