using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO.Primitives;

namespace iRacer.IO;
public class TelemetryDataReader : IDisposable
{
    private readonly ISimulatorDataAccessor _dataAccessor;
    private bool _isDisposed;

    public TelemetryDataReader(SimulatorDataConnection connection)
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));

        _dataAccessor = Connection.CreateDataAccessor();
    }

    public SimulatorDataConnection Connection { get; }

    public int GetActiveVariableBufferIndex()
    {
        var bufferCount = ReadVariableBufferCount();
        var variableBufferHeaders = ReadVariableBufferHeaders(bufferCount);

        int activeBufferIndex = 0;

        for (int i = 1; i < bufferCount; i++)
        {
            if (variableBufferHeaders[activeBufferIndex].TickCount < variableBufferHeaders[i].TickCount)
            {
                activeBufferIndex = i;
            }
        }

        return activeBufferIndex;
    }

    /// <summary>
    /// Reads the count of variable data buffers.
    /// </summary>
    /// <returns></returns>
    public int ReadVariableBufferCount()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableDataBufferCount(_dataAccessor.Span));
    }

    /// <summary>
    /// Reads the length of a variable data buffer.
    /// </summary>
    /// <returns></returns>
    public int ReadVariableBufferLength()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableDataBufferLength(_dataAccessor.Span));
    }

    public VariableBufferHeader[] ReadVariableBufferHeaders(int count)
    {
        if (count < 0 || count > DataFileConstants.MaxVariableBuffers)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                $"Value must be greater than or equal to zero and less than or equal to the constant {nameof(DataFileConstants)}.{nameof(DataFileConstants.MaxVariableBuffers)}.");
        }

        if (count == 0)
        {
            return Array.Empty<VariableBufferHeader>();
        }

        var headers = new VariableBufferHeader[count];
        var arraySpan = VariableDataSpans.VariableDataBufferHeaderArray(_dataAccessor.Span);

        for (int i = 0; i < count; i++)
        {
            var offset = i * DataFileConstants.VariableDataBufferHeaderLength;
            var headerSpan = arraySpan.Slice(offset, DataFileConstants.VariableDataBufferHeaderLength);

            headers[i] = MemoryMarshal.Read<VariableBufferHeader>(headerSpan);
        }

        return headers;
    }

    public VariableHeader[] ReadVariableHeaders()
    {
        var dataFileHeaderReader = _dataAccessor.CreateHeaderReader();

        // Read the count and offset to the array of headers
        var variableCount = dataFileHeaderReader.ReadVariableCount();
        var variableHeaderOffset = dataFileHeaderReader.ReadVariableHeaderOffset();

        // Read the headers
        return ReadVariableHeadersUnsafe(variableCount, variableHeaderOffset);
    }

    public VariableHeader[] ReadVariableHeaders(in DataFileHeader dataFileHeader)
    {
        return ReadVariableHeadersUnsafe(dataFileHeader.VariableCount, dataFileHeader.VariableHeaderOffset);
    }

    /// <summary>
    /// Reads variable headers without any local bounds checking.
    /// </summary>
    private VariableHeader[] ReadVariableHeadersUnsafe(int variableCount, int variableHeaderOffset)
    {
        var headers = new VariableHeader[variableCount];

        for (int i = 0; i < variableCount; i++)
        {
            var span = VariableDataSpans.VariableHeader(i, _dataAccessor.Span, variableHeaderOffset);

            headers[i] = MemoryMarshal.Read<VariableHeader>(span);
        }

        return headers;
    }

    public bool TryReceiveData(Memory<byte> memory, out int bytesRead, out int tickCount)
    {
        int activeBufferIndex = GetActiveVariableBufferIndex();

        return TryReceiveData(activeBufferIndex, memory, out bytesRead, out tickCount);
    }

    public bool TryReceiveData(int dataBufferIndex, Memory<byte> memory, out int bytesRead, out int tickCount)
    {
        if (dataBufferIndex < 0 || dataBufferIndex >= DataFileConstants.MaxVariableBuffers)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataBufferIndex),
                $"Value must be greater than or equal to zero and less than the constant {nameof(DataFileConstants)}.{nameof(DataFileConstants.MaxVariableBuffers)}.");
        }

        bytesRead = 0;
        tickCount = -1;

        if (_dataAccessor.CanRead)
        {
            return false;
        }

        var bufferLength = ReadVariableBufferLength();

        ref readonly var activeBufferHeader = ref MemoryMarshal.AsRef<VariableBufferHeader>(
            VariableDataSpans.VariableDataBufferHeader(dataBufferIndex, _dataAccessor.Span));

        // Copy variable data buffer
        var variableDataBufferSlice = _dataAccessor.Span.Slice(activeBufferHeader.BufferOffset, bufferLength);
        var outputBufferSlice = memory[..bufferLength];

        // As per the iRacing SDK, we will make two attempts to read the data, in case the data buffer was overwritten by the simulator
        // during our read.

        const int maxCopyAttempts = 2;
        int nCopyAttempt = 0;
        int ticks;
        bool copiedVariableDataBuffer;

        do
        {
            ticks = activeBufferHeader.TickCount;

            variableDataBufferSlice.CopyTo(outputBufferSlice.Span);

            copiedVariableDataBuffer = ticks == activeBufferHeader.TickCount;

            nCopyAttempt++;
        }
        while (!copiedVariableDataBuffer && nCopyAttempt < maxCopyAttempts);

        // Zero-out the buffer span if copying failed to prevent passing along bad data
        if (!copiedVariableDataBuffer)
        {
            outputBufferSlice.Span.Clear();
            return false;
        }

        bytesRead = outputBufferSlice.Length;
        tickCount = ticks;

        return true;
    }

    #region IDisposable Implementation

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _dataAccessor.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Implementation
}
