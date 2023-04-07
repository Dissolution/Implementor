namespace Jay.SourceGen.Signatures;

internal static class InsticExtensions
{
    public static void DeclareTo(this Instic instic, CodeBuilder codeBuilder)
    {
        if (instic == Instic.Static)
        {
            codeBuilder.Append("static ");
        }
    }
}