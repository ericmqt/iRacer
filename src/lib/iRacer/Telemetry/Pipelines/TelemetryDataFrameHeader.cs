using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace iRacer.Telemetry.Pipelines;
[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct TelemetryDataFrameHeader
{
    [FieldOffset(0)]
    public int Status;

    [FieldOffset(4)]
    public int Ticks;

    [FieldOffset(8)]
    public int DataLength;

    [FieldOffset(12)]
    public int VariableCount;
}
