// ReSharper disable RedundantUsingDirective
using System.Diagnostics;
using ConsoleApp.Interfaces;
using Implementor;

//Test test = new();
ITest? test = Implementation.OfITest();
// test.Id = 147;
// string str = test.Id.ToString();
// bool eq = test.Equals(new FakeTest() { Id = 147, Name = "Air" });
// Console.WriteLine($"Id = {str}, Eq(147,Air) = {eq}");

Console.WriteLine(test);
Debugger.Break();

// Nothing, for now...

//Console.WriteLine("Press Enter to quit");
//Console.ReadLine();
return 0;


internal sealed class FakeTest : ITest
{
    public int Id { get; set; }
    public string? Name { get; set; }
}