namespace iRacer.Telemetry;

internal abstract class TelemetryVariableDescriptor : ITelemetryVariableDescriptor
{
    protected TelemetryVariableDescriptor(TelemetryVariableDescriptorIndex descriptorIndex, string name, Type valueType)
    {
        DescriptorIndex = descriptorIndex;

        Name = !string.IsNullOrEmpty(name)
            ? name
            : throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
    }

    public TelemetryVariableDescriptorIndex DescriptorIndex { get; }
    public string Name { get; }
    public Type ValueType { get; }
}
