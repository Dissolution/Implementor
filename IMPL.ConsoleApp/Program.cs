using IMPL.Contracts;

//TestInterface test = new TestInterface();

//var type = test.GetType();
//Console.WriteLine(type);

Console.WriteLine("Press Enter to close.");
Console.ReadLine();


[Implement(Name = "Tester", Declaration = "internal")]
public interface ITestInterface
{
    int Id { get; }
}

//[Implement]
//public partial class TestClass
//{

//}