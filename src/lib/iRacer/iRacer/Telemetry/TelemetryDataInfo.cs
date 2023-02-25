using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO.Primitives;

namespace iRacer.Telemetry;
public class TelemetryDataInfo
{
    public TelemetryDataInfo(VariableHeader variableHeader)
    {
        Count = variableHeader.Count;
        CountAsTime = variableHeader.CountAsTime;
        Offset = variableHeader.Offset;
        Type = (TelemetryDataType)variableHeader.Type;

        Name = VariableHeader.GetNameString(variableHeader);
        Description = VariableHeader.GetDescriptionString(variableHeader);
        Unit = VariableHeader.GetUnitString(variableHeader);
    }

    public int Count { get; }
    public bool CountAsTime { get; }
    public string Description { get; }
    public string Name { get; }
    public int Offset { get; }
    public TelemetryDataType Type { get; }
    public string? Unit { get; }
}
