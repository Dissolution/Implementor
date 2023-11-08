using Implementor;

namespace ConsoleTester.Interfaces;

[Implement]
public interface ITest
{
    int Id { get; set; }
    string? Name { get; set; }
}