namespace iRacer.Telemetry;

public readonly struct TelemetryDataDescriptorIndex
{
    private readonly int _value;

    public TelemetryDataDescriptorIndex(int index)
    {
        _value = index;
    }

    public static implicit operator int(TelemetryDataDescriptorIndex descriptorIndex)
    {
        return descriptorIndex._value;
    }
}
