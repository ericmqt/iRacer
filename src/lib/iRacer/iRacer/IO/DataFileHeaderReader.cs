using System.Runtime.InteropServices;
using iRacer.IO.Primitives;

namespace iRacer.IO;
public readonly ref struct DataFileHeaderReader
{
    private readonly ReadOnlySpan<byte> _span;

    public DataFileHeaderReader(ReadOnlySpan<byte> span)
    {
        if (span.Length < DataFileConstants.HeaderLength)
        {
            throw new ArgumentException(
                $"Value '{nameof(span)}' has a length ({span.Length}) smaller than the header length ({DataFileConstants.HeaderLength}).", nameof(span));
        }

        _span = span;
    }

    public int ReadBufferCount()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.BufferCount, sizeof(int)));
    }

    public int ReadBufferLength()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.BufferLength, sizeof(int)));
    }

    public int ReadHeaderVersion()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.HeaderVersion, sizeof(int)));
    }

    public int ReadSessionInfoLength()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.SessionInfoLength, sizeof(int)));
    }

    public int ReadSessionInfoOffset()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.SessionInfoOffset, sizeof(int)));
    }

    public int ReadSessionInfoVersion()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.SessionInfoVersion, sizeof(int)));
    }

    public int ReadSimulatorStatus()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.Status, sizeof(int)));
    }

    public int ReadTickRate()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.TickRate, sizeof(int)));
    }

    public int ReadVariableCount()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.VariableCount, sizeof(int)));
    }

    public int ReadVariableHeaderOffset()
    {
        return MemoryMarshal.Read<int>(
            _span.Slice(DataFileHeaderFieldOffsets.VariableHeaderOffset, sizeof(int)));
    }
}
