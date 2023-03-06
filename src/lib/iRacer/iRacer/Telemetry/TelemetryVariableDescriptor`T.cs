namespace iRacer.Telemetry;

internal sealed class TelemetryVariableDescriptor<T> : TelemetryVariableDescriptor, ITelemetryVariableDescriptor<T>
    where T : unmanaged
{
    internal TelemetryVariableDescriptor(TelemetryVariableDescriptorIndex descriptorIndex, string name)
        : base(descriptorIndex, name, typeof(T))
    {
    }
}
