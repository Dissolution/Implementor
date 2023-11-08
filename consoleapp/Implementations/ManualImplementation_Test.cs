using ConsoleTester.Interfaces;

namespace ConsoleTester.Implementations;

public class ManualImplementation_Test : ITest
{
    private int _id;
    private string? _name;
    
    public int Id
    {
        get => _id;
        set => _id = value;
    }
    public string? Name
    {
        get => _name;
        set => _name = value;
    }

    public ManualImplementation_Test()
    {
        
    }
}