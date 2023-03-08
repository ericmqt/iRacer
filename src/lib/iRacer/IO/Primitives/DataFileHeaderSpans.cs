using System.Runtime.CompilerServices;

namespace iRacer.IO.Primitives;

public static class DataFileHeaderSpans
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> DataFileHeader(ReadOnlySpan<byte> data)
    {
        return data[..DataFileConstants.HeaderLength];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> HeaderVersion(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.HeaderVersion, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> SessionInfoLength(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.SessionInfoLength, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> SessionInfoOffset(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.SessionInfoOffset, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> SessionInfoVersion(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.SessionInfoVersion, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> Status(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.Status, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> TickRate(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.TickRate, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableCount(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.VariableCount, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableDataBufferCount(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.BufferCount, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableDataBufferLength(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.BufferLength, 4);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableHeaderOffset(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileHeaderFieldOffsets.VariableHeaderOffset, 4);
    }
}
