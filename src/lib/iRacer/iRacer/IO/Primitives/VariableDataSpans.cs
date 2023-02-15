using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace iRacer.IO.Primitives;
public static class VariableDataSpans
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableDataBufferHeader(int index, ReadOnlySpan<byte> data)
    {
        if (index < 0 || index >= DataFileConstants.MaxVariableBuffers)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                $"Value must be greater than or equal to zero and less than the constant {nameof(DataFileConstants)}.{nameof(DataFileConstants.MaxVariableBuffers)}.");
        }

        var offset = DataFileConstants.VariableDataBufferHeaderArrayOffset + (index * DataFileConstants.VariableDataBufferHeaderLength);

        return data.Slice(offset, DataFileConstants.VariableDataBufferHeaderLength);
    }

    /// <summary>
    /// Gets a span for the array of <see cref="VariableBufferHeader"/> values.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableDataBufferHeaderArray(ReadOnlySpan<byte> data)
    {
        return data.Slice(DataFileConstants.VariableDataBufferHeaderArrayOffset, DataFileConstants.VariableBufferHeaderArrayLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableHeader(int index, ReadOnlySpan<byte> data, in DataFileHeader dataFileHeader)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"'{nameof(index)}' must be greater than or equal to zero.");
        }

        var lastVariableIndex = dataFileHeader.VariableCount > 0 ? dataFileHeader.VariableCount - 1 : 0;

        if (index > lastVariableIndex)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index), $"Value of '{nameof(index)}' ({index})  is beyond the number of variables in the header.");
        }

        var offset = dataFileHeader.VariableHeaderOffset + (index * DataFileConstants.VariableHeaderLength);

        return data.Slice(offset, DataFileConstants.VariableHeaderLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> VariableHeader(int index, ReadOnlySpan<byte> data, int variableHeaderOffset)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"'{nameof(index)}' must be greater than or equal to zero.");
        }

        if (variableHeaderOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(variableHeaderOffset), $"'{nameof(variableHeaderOffset)}' must be greater than or equal to zero.");
        }

        var offset = variableHeaderOffset + (index * DataFileConstants.VariableHeaderLength);

        return data.Slice(offset, DataFileConstants.VariableHeaderLength);
    }
}
