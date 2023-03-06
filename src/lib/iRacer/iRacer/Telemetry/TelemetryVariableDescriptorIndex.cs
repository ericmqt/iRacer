namespace iRacer.Telemetry;

public readonly struct TelemetryVariableDescriptorIndex : IComparable<TelemetryVariableDescriptorIndex>, IEquatable<TelemetryVariableDescriptorIndex>
{
    private readonly int _value;

    public TelemetryVariableDescriptorIndex(int index)
    {
        _value = index;
    }
    public int CompareTo(TelemetryVariableDescriptorIndex other)
    {
        return _value.CompareTo(other._value);
    }


    public bool Equals(TelemetryVariableDescriptorIndex other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is TelemetryVariableDescriptorIndex otherIndex && Equals(otherIndex);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_value);
    }

    public static implicit operator int(TelemetryVariableDescriptorIndex descriptorIndex)
    {
        return descriptorIndex._value;
    }

    public static bool operator ==(TelemetryVariableDescriptorIndex left, TelemetryVariableDescriptorIndex right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TelemetryVariableDescriptorIndex left, TelemetryVariableDescriptorIndex right)
    {
        return !(left == right);
    }

    public static bool operator <(TelemetryVariableDescriptorIndex left, TelemetryVariableDescriptorIndex right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(TelemetryVariableDescriptorIndex left, TelemetryVariableDescriptorIndex right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(TelemetryVariableDescriptorIndex left, TelemetryVariableDescriptorIndex right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(TelemetryVariableDescriptorIndex left, TelemetryVariableDescriptorIndex right)
    {
        return left.CompareTo(right) >= 0;
    }
}
