namespace Jay.SourceGen.Coding;

public interface ICodeWritable
{
    void Write(CodeBuilder codeBuilder);
}


public static class CodeWritableExtensions
{
    public static CodeBuilder Append<TWritable>(this CodeBuilder codeBuilder, TWritable value)
        where TWritable : ICodeWritable
    {
        value.Write(codeBuilder);
        return codeBuilder;
    }
}