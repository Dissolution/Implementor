using Jay.SourceGen.Text;

namespace Jay.SourceGen.Logging;

public static class DbgLog
{
    [Conditional("DEBUG")]
    public static void WriteLine(NonFormattableString text)
    {
        Debug.WriteLine($"{DateTime.Now:HH\\:mm\\:ss\\.ff}: {text}");
    }

    [Conditional("DEBUG")]
    public static void WriteLine(FormattableString text)
    {
        string format = text.Format;
        if (text.ArgumentCount == 0)
        {
            Debug.WriteLine($"{DateTime.Now:HH\\:mm\\:ss\\.ff}: {format}");
        }
        else
        {
            var args = text.GetArguments();
            // Process
            Debugger.Break();
            Debug.WriteLine($"{DateTime.Now:HH\\:mm\\:ss\\.ff}: {string.Format(format, args)}");
        }
    }
}
