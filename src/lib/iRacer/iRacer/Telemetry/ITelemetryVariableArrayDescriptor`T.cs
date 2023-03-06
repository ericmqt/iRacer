namespace iRacer.Telemetry;

public interface ITelemetryVariableArrayDescriptor<T> : ITelemetryVariableDescriptor
    where T : unmanaged
{
    int ArrayLength { get; }
}
