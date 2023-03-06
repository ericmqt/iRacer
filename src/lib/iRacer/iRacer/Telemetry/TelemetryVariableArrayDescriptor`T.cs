namespace iRacer.Telemetry;

internal sealed class TelemetryVariableArrayDescriptor<T> : TelemetryVariableDescriptor, ITelemetryVariableArrayDescriptor<T>
    where T : unmanaged
{
    internal TelemetryVariableArrayDescriptor(TelemetryVariableDescriptorIndex descriptorIndex, string name, Type valueType)
        : base(descriptorIndex, name, valueType)
    {
    }

    public int ArrayLength { get; }
}
