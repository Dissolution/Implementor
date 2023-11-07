using ConsoleTester.Interfaces;

namespace ConsoleTester.Implementations;

public class ManualImplementation_Test : ITest
{
    private int _id;

    public int Id
    {
        get => _id;
        set => _id = value;
    }
}