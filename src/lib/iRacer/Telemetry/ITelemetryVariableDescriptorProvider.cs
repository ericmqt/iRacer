namespace iRacer.Telemetry;

public interface ITelemetryVariableDescriptorProvider : IReadOnlyCollection<ITelemetryVariableDescriptor>
{
    ITelemetryVariableDescriptor this[int index] { get; }
}
