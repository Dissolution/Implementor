using Microsoft.CodeAnalysis;

namespace Jay.SourceGen.Signatures;

public sealed class EventSig : MemberSig, IEquatable<EventSig>, IEquatable<IEventSymbol>, IEquatable<EventInfo>
{
    public static bool operator ==(EventSig? left, EventSig? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }
    public static bool operator !=(EventSig? left, EventSig? right)
    {
        if (ReferenceEquals(left, right)) return false;
        if (left is null || right is null) return true;
        return !left.Equals(right);
    }

    public TypeSig EventType { get; set; }
    public MethodSig? AddMethod { get; set; } = null;
    public MethodSig? RemoveMethod { get; set; } = null;
    public MethodSig? RaiseMethod { get; set; } = null;

    public EventSig(IEventSymbol eventSymbol)
        : base(eventSymbol)
    {
        this.MemberType = MemberTypes.Event;
        this.EventType = new(eventSymbol.Type);

        // Adder
        var addMethod = eventSymbol.AddMethod;
        if (addMethod is not null)
            this.AddMethod = new(addMethod);
        // Remover
        var removeMethod = eventSymbol.RemoveMethod;
        if (removeMethod is not null)
            this.RemoveMethod = new(removeMethod);
        // Raiser
        var raiseMethod = eventSymbol.RaiseMethod;
        if (raiseMethod is not null)
            this.RaiseMethod = new(raiseMethod);
    }

    public EventSig(EventInfo eventInfo)
        : base(eventInfo)
    {
        this.MemberType = MemberTypes.Event;
        this.EventType = new(eventInfo.EventHandlerType);

        // Adder
        var addMethod = eventInfo.AddMethod;
        if (addMethod is not null)
            this.AddMethod = new(addMethod);
        // Remover
        var removeMethod = eventInfo.RemoveMethod;
        if (removeMethod is not null)
            this.RemoveMethod = new(removeMethod);
        // Raiser
        var raiseMethod = eventInfo.RaiseMethod;
        if (raiseMethod is not null)
            this.RaiseMethod = new(raiseMethod);
    }

    public bool Equals(EventSig? propertySig)
    {
        return propertySig is not null &&
            FullName == propertySig.FullName &&
            EventType.Equals(propertySig.EventType);
    }

    public bool Equals(IEventSymbol? propertySymbol)
    {
        return propertySymbol is not null &&
           FullName == propertySymbol.GetFullName() &&
           EventType.Equals(propertySymbol.Type);
    }

    public bool Equals(EventInfo? propertyInfo)
    {
        return propertyInfo is not null &&
           FullName == $"{propertyInfo.ReflectedType.FullName}.{propertyInfo.Name}" &&
           EventType.Equals(propertyInfo.EventHandlerType);
    }

    public override bool Equals(MemberSig? memberSig)
    {
        return memberSig is EventSig eventSig && Equals(eventSig);
    }

    public override bool Equals(ISymbol? symbol)
    {
        return symbol is IEventSymbol eventSymbol && Equals(eventSymbol);
    }

    public override bool Equals(MemberInfo? member)
    {
        return member is EventInfo eventInfo && Equals(eventInfo);
    }

    public override bool Equals(object? obj)
    {
        if (obj is EventSig eventSig) return Equals(eventSig);
        if (obj is IEventSymbol eventSymbol) return Equals(eventSymbol);
        if (obj is EventInfo eventInfo) return Equals(eventInfo);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Create(FullName, EventType);
    }

    public override string ToString()
    {
        return $$"""
            [{{string.Join(", ", Attributes)}}]
            {{Visibility}} {{Instic}} {{Keywords}} event {{FullName}};
            """;
    }
}