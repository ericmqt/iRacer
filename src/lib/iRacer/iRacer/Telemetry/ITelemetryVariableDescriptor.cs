namespace iRacer.Telemetry;

public interface ITelemetryVariableDescriptor
{
    TelemetryVariableDescriptorIndex DescriptorIndex { get; }
    string Name { get; }
    Type ValueType { get; }
}
