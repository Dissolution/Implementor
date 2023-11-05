using System.Buffers;

namespace Implementor.Text;

public sealed class IndentManager : IDisposable
{
    private char[] _indents;
    private int _indentsPosition;
    private readonly Stack<int> _indentOffsets;

    public ReadOnlySpan<char> CurrentIndent => _indents.AsSpan(0, _indentsPosition);
    
    public IndentManager()
    {
        _indents = ArrayPool<char>.Shared.Rent(1024);
        _indentsPosition = 0;
        _indentOffsets = new(0);
    }
    
    public void AddIndent(char indent)
    {
        int pos = _indentsPosition;
        int newPos = pos + 1;
        if (newPos > _indents.Length)
            throw new InvalidOperationException();
        _indentOffsets.Push(pos);
        _indents[pos] = indent;
        _indentsPosition = newPos;
    }

    public void AddIndent(string? indent) => AddIndent(indent.AsSpan());
    

    public void AddIndent(scoped ReadOnlySpan<char> indent)
    {
        int pos = _indentsPosition;
        int newPos = pos + indent.Length;
        if (newPos > _indents.Length)
            throw new InvalidOperationException();
        _indentOffsets.Push(pos);
        indent.CopyTo(_indents.AsSpan(pos));
        _indentsPosition = newPos;
    }

    public void RemoveIndent()
    {
        if (_indentOffsets.Count > 0)
        {
            _indentsPosition = _indentOffsets.Pop();
        }
    }

    public void RemoveIndent(out ReadOnlySpan<char> lastIndent)
    {
        if (_indentOffsets.Count > 0)
        {
            var lastIndentIndex = _indentOffsets.Pop();
            lastIndent = _indents.AsSpan()[new Range(start: lastIndentIndex, end: _indentsPosition)];
            _indentsPosition = lastIndentIndex;
        }
        else
        {
            lastIndent = default;
        }
    }

    
    public void Dispose()
    {
        char[]? toReturn = _indents;
        _indents = null!;
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}