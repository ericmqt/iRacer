using System.Runtime.InteropServices;

namespace iRacer.IO.Primitives;

[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.VariableDataBufferHeaderLength)]
public struct VariableBufferHeader
{
    /// <summary>
    /// Used to detect changes in data.
    /// </summary>
    [FieldOffset(VariableBufferHeaderOffsets.TickCount)]
    public int TickCount;

    [FieldOffset(VariableBufferHeaderOffsets.BufferOffset)]
    public int BufferOffset;

    [FieldOffset(VariableBufferHeaderOffsets.Padding)]
    public ulong Padding;
}