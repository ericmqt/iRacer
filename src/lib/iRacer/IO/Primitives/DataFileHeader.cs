using System.Runtime.InteropServices;

namespace iRacer.IO.Primitives;

[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.HeaderLength)]
public struct DataFileHeader
{
    public DataFileHeader()
    {

    }

    [FieldOffset(DataFileHeaderFieldOffsets.HeaderVersion)]
    public int HeaderVersion;

    [FieldOffset(DataFileHeaderFieldOffsets.Status)]
    public int Status;

    [FieldOffset(DataFileHeaderFieldOffsets.TickRate)]
    public int TickRate;

    [FieldOffset(DataFileHeaderFieldOffsets.SessionInfoVersion)]
    public int SessionInfoVersion;

    [FieldOffset(DataFileHeaderFieldOffsets.SessionInfoLength)]
    public int SessionInfoLength;

    [FieldOffset(DataFileHeaderFieldOffsets.SessionInfoOffset)]
    public int SessionInfoOffset;

    [FieldOffset(DataFileHeaderFieldOffsets.VariableCount)]
    public int VariableCount;

    [FieldOffset(DataFileHeaderFieldOffsets.VariableHeaderOffset)]
    public int VariableHeaderOffset;

    [FieldOffset(DataFileHeaderFieldOffsets.BufferCount)]
    public int BufferCount;

    [FieldOffset(DataFileHeaderFieldOffsets.BufferLength)]
    public int BufferLength;

    [FieldOffset(DataFileHeaderFieldOffsets.Padding)]
    public ulong Padding;
}