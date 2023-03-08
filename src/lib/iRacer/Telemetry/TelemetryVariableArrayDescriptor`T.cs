namespace iRacer.Telemetry;

internal sealed class TelemetryVariableArrayDescriptor<T> : TelemetryVariableDescriptor, ITelemetryVariableArrayDescriptor<T>
    where T : unmanaged
{
    internal TelemetryVariableArrayDescriptor(TelemetryVariableDescriptorIndex descriptorIndex, string name, int arrayLength)
        : base(descriptorIndex, name, typeof(T))
    {
        ArrayLength = arrayLength;
    }

    public int ArrayLength { get; }
}
