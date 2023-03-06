namespace iRacer.Telemetry;

public readonly struct TelemetryVariableDescriptorIndex
{
    private readonly int _value;

    public TelemetryVariableDescriptorIndex(int index)
    {
        _value = index;
    }

    public static implicit operator int(TelemetryVariableDescriptorIndex descriptorIndex)
    {
        return descriptorIndex._value;
    }
}
