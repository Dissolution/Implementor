namespace Implementor.Scratch;

public sealed class Parameters : List<ParameterSignature>
{
    public static Parameters From(ImmutableArray<IParameterSymbol> parameterSymbols)
    {
        var parameters = new Parameters();
        if (!parameterSymbols.IsDefaultOrEmpty)
        {
            parameters.AddRange(parameterSymbols
                    .Select(static p => ParameterSignature.Create(p))
                    .Where(static p => p is not null)!);
        }

        return parameters;
    }
}