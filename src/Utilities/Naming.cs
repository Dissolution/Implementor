namespace Implementor.Utilities;

internal static class Naming
{
    public static string GetImplementationName(string interfaceName)
    {
        if (interfaceName.StartsWith("I"))
            return interfaceName.Substring(1);
        else
            return "impl_" + interfaceName;
    }
}