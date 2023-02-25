namespace iRacer.Telemetry;

public interface ITelemetryDataDescriptor
{
    TelemetryDataDescriptorIndex DescriptorIndex { get; }
    string Name { get; }
    Type ValueType { get; }
}
