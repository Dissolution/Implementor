using System.Diagnostics;
using Implementor;
using ConsoleTester.Interfaces;
//
// //Test test = new();
// ITest test = Implementation.OfITest();
// test.Id = 147;
// string str = test.Id.ToString();
// bool eq = test.Equals(new FakeTest() { Id = 147, Name = "Air" });
// Console.WriteLine($"Id = {str}, Eq(147,Air) = {eq}");
//
// Debugger.Break();

// Nothing, for now...

Console.WriteLine("Press Enter to quit");
Console.ReadLine();
return 0;


internal sealed class FakeTest : ITest
{
    public int Id { get; set; }
    public string? Name { get; set; }
}