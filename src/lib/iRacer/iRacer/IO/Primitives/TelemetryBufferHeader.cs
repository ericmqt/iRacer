using System.Runtime.InteropServices;

namespace iRacer.IO.Primitives;

[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.VariableDataBufferHeaderLength)]
public struct TelemetryBufferHeader
{
    /// <summary>
    /// Used to detect changes in data.
    /// </summary>
    [FieldOffset(TelemetryBufferHeaderOffsets.TickCount)]
    public int TickCount;

    [FieldOffset(TelemetryBufferHeaderOffsets.BufferOffset)]
    public int BufferOffset;

    [FieldOffset(TelemetryBufferHeaderOffsets.Padding)]
    public ulong Padding;
}