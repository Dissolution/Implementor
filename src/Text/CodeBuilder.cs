using System.Buffers;
using System.Diagnostics;

namespace Implementor.Text;

/// <summary>
/// <b>C</b>ode <b>B</b>uilder <b>A</b>ction<br/>
/// <see cref="Action{T}">Action&lt;CodeBuilder&gt;</see>
/// </summary>
public delegate void CBA(CodeBuilder codeBuilder);

public sealed class CodeBuilder : IDisposable
{
    public static string DefaultNewLine { get; set; } = SyntaxFactory.ElasticCarriageReturnLineFeed.ToFullString();
    public static CodeBuilder New => new();


    private IndentManager _indentManager = new();
    private char[] _chars = ArrayPool<char>.Shared.Rent(128 * 1024);
    private int _length = 0;

    internal Span<char> Written => _chars.AsSpan(0, _length);
    internal Span<char> Available => _chars.AsSpan(_length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int growCount)
    {
        Debug.Assert(growCount > 0);
        char[] newArray = ArrayPool<char>.Shared.Rent(2 * (_chars.Length + growCount));
        _chars.AsSpan(0, _length).CopyTo(newArray);
        char[] toReturn = _chars;
        _chars = newArray;
        ArrayPool<char>.Shared.Return(toReturn);
    }


    /// <summary>
    /// Gets the NewLine + Indent that exists at the current position
    /// </summary>
    /// <returns></returns>
    private ReadOnlySpan<char> GetCurrentIndent()
    {
        var newLine = DefaultNewLine.AsSpan();
        var written = this.Written;
        var i = written.LastIndexOf<char>(newLine);
        if (i == -1)
        {
            return default;
        }
        else
        {
            return written.Slice(i + newLine.Length);
        }
    }

    internal void IndentAwareAction(CBA cba)
    {
        var newIndent = GetCurrentIndent();
        _indentManager.AddIndent(newIndent);
        cba(this);
#if DEBUG
        _indentManager.RemoveIndent(out var removedIndent);
        if (!newIndent.SequenceEqual(removedIndent))
        {
            var ni = newIndent.ToString();
            var ri = removedIndent.ToString();
            Debugger.Break();
        }
        //Debug.Assert(newIndent.SequenceEqual(removedIndent));
#else
        _indentManager.RemoveIndent();
#endif
    }

    internal void WriteIndentAwareText(ReadOnlySpan<char> text)
    {
        var newLine = DefaultNewLine;
        var newIndent = GetCurrentIndent();
        _indentManager.AddIndent(newIndent);
        
        // Replace embedded NewLines with NewLine+Indent
        var split = text.TextSplit(newLine);
        while (split.MoveNext())
        {
            this.Append(split.Text);
            while (split.MoveNext())
            {
                this.NewLine().Append(split.Text);
            }
        }
#if DEBUG
        _indentManager.RemoveIndent(out var removedIndent);
        Debug.Assert(newIndent.SequenceEqual(removedIndent));
#else
        _indentManager.RemoveIndent();
#endif
    }

    internal void WriteIndentAwareValue<T>([AllowNull] T value)
    {
        switch (value)
        {
            case null:
            {
                return;
            }
            case string str:
            {
                WriteIndentAwareText(str.AsSpan());
                return;
            }
            case IFormattable formattable:
            {
                WriteIndentAwareText(formattable.ToString(default, default).AsSpan());
                return;
            }
            case CBA codeBuild:
            {
                IndentAwareAction(codeBuild);
                return;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public CodeBuilder Append(char ch)
    {
        int pos = _length;
        if (pos >= _chars.Length)
        {
            GrowBy(1);
        }

        _chars[pos] = ch;
        _length = pos + 1;
        return this;
    }

    public CodeBuilder Append(scoped ReadOnlySpan<char> text)
    {
        int textLen = text.Length;
        int pos = _length;
        if (pos + textLen > _chars.Length)
        {
            GrowBy(textLen);
        }

        text.CopyTo(_chars.AsSpan(pos));
        _length = pos + textLen;
        return this;
    }

    public CodeBuilder Append(string? str)
    {
        if (str is not null)
        {
            int textLen = str.Length;
            int pos = _length;
            if (pos + textLen > _chars.Length)
            {
                GrowBy(textLen);
            }

            str.AsSpan().CopyTo(_chars.AsSpan(pos));
            _length = pos + textLen;
        }

        return this;
    }

    public CodeBuilder Append([InterpolatedStringHandlerArgument("")] ref InterpolatedCode interpolatedCode)
    {
        // Will already have appended, as we pass this to the InterpolatedCode
        return this;
    }

    public CodeBuilder Append<T>([AllowNull] T value)
    {
        // Intercept Code Builder Actions
        if (value is CBA codeBuilderAction)
        {
            IndentAwareAction(codeBuilderAction);
            return this;
        }
        // Use the ToCodeHelper
        ToCodeHelper.WriteValueTo<T>(value, this);
        return this;
    }

    public CodeBuilder AppendIf<T>(T? value, char spacer)
    {
        // Intercept Code Builder Actions
        if (value is CBA codeBuilderAction)
        {
            int pos = _length;
            IndentAwareAction(codeBuilderAction);
            if (_length != pos)
            {
                return Append(spacer);
            }
            return this;
        }
        // Use the ToCodeHelper
        if (ToCodeHelper.WriteValueTo<T>(value, this))
        {
            return Append(spacer);
        }
        return this;
    }

    public CodeBuilder Append<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        return Append(str);
    }

    public CodeBuilder Append<T>(T? value, Casing casing)
        => Append(value?.ToString().ToCase(casing));

    public CodeBuilder NewLine()
    {
        return Append(DefaultNewLine).Append(_indentManager.CurrentIndent);
    }

    /// <summary>
    /// Writes the given <paramref name="comment"/> as a comment line / lines
    /// </summary>
    public CodeBuilder Comment(string? comment)
    {
        /* Most of the time, this is probably a single line.
         * But we do want to watch out for newline characters to turn
         * this into a multi-line comment */

        var comments = comment.TextSplit(DefaultNewLine).GetEnumerator();
        if (!comments.MoveNext())
        {
            // Null or empty comment is blank
            return this;
        }

        // Capture this comment
        var firstComment = comments.Text;
        if (!comments.MoveNext())
        {
            // Only a single comment
            return Append("// ").Append(firstComment).NewLine();
        }

        // Multiple comments

        // Write the first one we captured before
        Append("/* ").Append(firstComment).NewLine();

        // Write this comment
        Append(" * ").Append(comments.Text).NewLine();

        // Write the rest
        while (comments.MoveNext())
        {
            Append(" * ").Append(comments.Text).NewLine();
        }

        // Close the block
        return Append(" */").NewLine();
    }

    public CodeBuilder Comment(string? comment, CommentType commentType)
    {
        switch (commentType)
        {
            case CommentType.Any:
            {
                return this.Comment(comment);
            }
            case CommentType.SingleLine:
            {
                foreach (var line in comment.TextSplit(DefaultNewLine))
                {
                    Append("// ").Append(line).NewLine();
                }

                return this;
            }
            case CommentType.Xml:
            {
                foreach (var line in comment.TextSplit(DefaultNewLine))
                {
                    Append("/// ").Append(line).NewLine();
                }

                return this;
            }
            case CommentType.MultiLine:
            {
                var comments = comment.TextSplit(DefaultNewLine).GetEnumerator();
                if (!comments.MoveNext())
                {
                    // Null or empty comment is blank
                    return Append("/* */").NewLine();
                }

                // Capture this comment
                var firstComment = comments.Text;
                if (!comments.MoveNext())
                {
                    // Only a single comment
                    return Append("/* ")
                        .Append(firstComment)
                        .Append(" */")
                        .NewLine();
                }

                // Multiple comments

                // Write the first one we captured before
                Append("/* ").Append(firstComment).NewLine();

                // Write this comment
                Append(" * ").Append(comments.Text).NewLine();

                // Write the rest
                while (comments.MoveNext())
                {
                    Append(" * ").Append(comments.Text).NewLine();
                }

                // Close the block
                return Append(" */").NewLine();
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(commentType));
        }
    }

    public CodeBuilder Enumerate<T>(IEnumerable<T> values, Action<CodeBuilder, T> perValue)
    {
        foreach (var value in values)
        {
            perValue(this, value);
        }

        return this;
    }

    public CodeBuilder Iterate<T>(IEnumerable<T> values, Action<CodeBuilder, T, int> perValueIndex)
    {
        if (values is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                perValueIndex(this, list[i], i);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            int i = 0;
            perValueIndex(this, e.Current, i);
            while (e.MoveNext())
            {
                i++;
                perValueIndex(this, e.Current, i);
            }
        }

        return this;
    }

    public CodeBuilder Delimit<T>(Action<CodeBuilder> delimit, IEnumerable<T> values, Action<CodeBuilder, T> perValue)
    {
        if (values is IList<T> list)
        {
            var count = list.Count;
            if (count == 0)
                return this;

            perValue(this, list[0]);
            for (var i = 1; i < count; i++)
            {
                delimit(this);
                perValue(this, list[i]);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            perValue(this, e.Current);
            while (e.MoveNext())
            {
                delimit(this);
                perValue(this, e.Current);
            }
        }

        return this;
    }

    public CodeBuilder Delimit<T>(string delimiter, IEnumerable<T> values, Action<CodeBuilder, T> perValue)
        => this.Delimit<T>(b => b.Append(delimiter), values, perValue);

    public CodeBuilder If(bool result, Action<CodeBuilder>? ifTrue, Action<CodeBuilder>? ifFalse = null)
    {
        if (result)
        {
            ifTrue?.Invoke(this);
        }
        else
        {
            ifFalse?.Invoke(this);
        }

        return this;
    }

    public CodeBuilder Invoke(Action<CodeBuilder> build)
    {
        build(this);
        return this;
    }


    public void Clear()
    {
        _length = 0;
    }

    public void Dispose()
    {
        char[]? toReturn = _chars;
        _chars = null!;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString()
    {
        return new string(_chars, 0, _length);
    }
}