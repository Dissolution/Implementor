namespace Jay.SourceGen.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T value)
    {
        foreach (var item in source)
        {
            yield return item;
        }
        yield return value;
    }

    public static void Consume<T>(this IEnumerable<T> source, Action<T> perValue)
    {
        foreach (var value in source)
        {
            perValue(value);
        }
    }

    public static T? OneOrDefault<T>(this IEnumerable<T>? source, T? fallback = default)
    {
        if (source is null) return fallback;
        using var e = source.GetEnumerator();
        if (!e.MoveNext()) return fallback;
        T value = e.Current;
        if (e.MoveNext()) return fallback;
        return value;
    }
}
