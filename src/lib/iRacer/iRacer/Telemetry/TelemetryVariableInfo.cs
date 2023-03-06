using iRacer.IO.Primitives;

namespace iRacer.Telemetry;

public class TelemetryVariableInfo
{
    public TelemetryVariableInfo(TelemetryVariableHeader variableHeader)
    {
        Count = variableHeader.Count;
        CountAsTime = variableHeader.CountAsTime;
        Offset = variableHeader.Offset;
        Type = (TelemetryVariableValueType)variableHeader.Type;

        Name = TelemetryVariableHeader.GetNameString(variableHeader);
        Description = TelemetryVariableHeader.GetDescriptionString(variableHeader);
        Unit = TelemetryVariableHeader.GetUnitString(variableHeader);
    }

    public int Count { get; }
    public bool CountAsTime { get; }
    public string Description { get; }
    public string Name { get; }
    public int Offset { get; }
    public TelemetryVariableValueType Type { get; }
    public string? Unit { get; }
}
