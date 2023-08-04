namespace Jay.SourceGen.Validation;

public static class Extensions
{
    [return: NotNull]
    public static T ThrowIfNull<T>(
        [AllowNull, NotNull] this T value,
        string? exMessage = null,
        [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        if (value is not null) return value!;
        throw new ArgumentNullException(valueName, exMessage);
    }
}
