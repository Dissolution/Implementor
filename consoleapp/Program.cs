
using System.Diagnostics;
using Implementor;
using ConsoleTester.Interfaces;

//Test test = new();
ITest test = Implementation.OfITest();
string? str = test?.Id.ToString();
Console.WriteLine(str);

Debugger.Break();

// Nothing, for now...

Console.WriteLine("Press Enter to quit");
Console.ReadLine();
return 0;