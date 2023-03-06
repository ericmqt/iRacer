namespace iRacer.Telemetry;

internal sealed class TelemetryVariableDescriptor<T> : TelemetryVariableDescriptor, ITelemetryVariableDescriptor<T>
    where T : unmanaged
{
    internal TelemetryVariableDescriptor(TelemetryVariableDescriptorIndex descriptorIndex, string name, Type valueType)
        : base(descriptorIndex, name, valueType)
    {
    }
}
