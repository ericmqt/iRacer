using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO.Primitives;

namespace iRacer.IO;
public class SimulatorDataReader : IDisposable
{
    private readonly ISimulatorDataAccessor _dataAccessor;
    private bool _isDisposed;

    public SimulatorDataReader(SimulatorDataConnection connection)
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));

        _dataAccessor = Connection.CreateDataAccessor();
    }

    public SimulatorDataConnection Connection { get; }

    public int GetActiveTelemetryBufferIndex()
    {
        var bufferCount = ReadTelemetryBufferCount();

        int activeBufferIndex = -1;
        int activeBufferTickCount = -1;

        for (int i = 0; i < bufferCount; i++)
        {
            var tickCount = MemoryMarshal.Read<int>(TelemetryDataSpans.TelemetryBufferHeaderTickCount(i, _dataAccessor.Span));

            if (tickCount > activeBufferTickCount)
            {
                activeBufferIndex = i;
                activeBufferTickCount = tickCount;
            }
        }

        return activeBufferIndex;
    }

    public string ReadSessionInfoString()
    {
        var offset = MemoryMarshal.Read<int>(DataFileHeaderSpans.SessionInfoOffset(_dataAccessor.Span));
        var length = MemoryMarshal.Read<int>(DataFileHeaderSpans.SessionInfoLength(_dataAccessor.Span));

        var sessionInfoBytes = _dataAccessor.Span.Slice(offset, length);

        return Encoding.UTF8.GetString(sessionInfoBytes);
    }

    public int ReadSimulatorStatus()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.Status(_dataAccessor.Span));
    }

    /// <summary>
    /// Reads the number of telemetry data buffers.
    /// </summary>
    /// <returns></returns>
    public int ReadTelemetryBufferCount()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableDataBufferCount(_dataAccessor.Span));
    }

    public TelemetryBufferHeader[] ReadTelemetryBufferHeaders(int count)
    {
        if (count < 0 || count > DataFileConstants.MaxVariableBuffers)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                $"Value must be greater than or equal to zero and less than or equal to the constant {nameof(DataFileConstants)}.{nameof(DataFileConstants.MaxVariableBuffers)}.");
        }

        if (count == 0)
        {
            return Array.Empty<TelemetryBufferHeader>();
        }

        var headers = new TelemetryBufferHeader[count];

        for (int i = 0; i < count; i++)
        {
            var headerSpan = TelemetryDataSpans.TelemetryBufferHeader(i, _dataAccessor.Span);

            headers[i] = MemoryMarshal.Read<TelemetryBufferHeader>(headerSpan);
        }

        return headers;
    }

    /// <summary>
    /// Reads the length of a single telemetry data buffer.
    /// </summary>
    /// <returns></returns>
    public int ReadTelemetryBufferLength()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableDataBufferLength(_dataAccessor.Span));
    }

    public int ReadTelemetryVariableCount()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableCount(_dataAccessor.Span));
    }

    public TelemetryVariableHeader[] ReadTelemetryVariableHeaders()
    {
        var variableCount = MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableCount(_dataAccessor.Span));
        var variableHeaderOffset = MemoryMarshal.Read<int>(DataFileHeaderSpans.VariableHeaderOffset(_dataAccessor.Span));

        // Read the headers
        return ReadTelemetryVariableHeadersUnsafe(variableCount, variableHeaderOffset);
    }

    /// <summary>
    /// Reads variable headers without bounds checking.
    /// </summary>
    private TelemetryVariableHeader[] ReadTelemetryVariableHeadersUnsafe(int variableCount, int variableHeaderOffset)
    {
        var headers = new TelemetryVariableHeader[variableCount];

        for (int i = 0; i < variableCount; i++)
        {
            var span = TelemetryDataSpans.VariableHeader(i, _dataAccessor.Span, variableHeaderOffset);

            headers[i] = MemoryMarshal.Read<TelemetryVariableHeader>(span);
        }

        return headers;
    }

    public bool TryReceiveTelemetryData(Memory<byte> memory, out int bytesRead, out int tickCount)
    {
        int activeBufferIndex = GetActiveTelemetryBufferIndex();

        return TryReceiveTelemetryData(activeBufferIndex, memory, out bytesRead, out tickCount);
    }

    public bool TryReceiveTelemetryData(int dataBufferIndex, Memory<byte> memory, out int bytesRead, out int tickCount)
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

        var bufferLength = ReadTelemetryBufferLength();

        ref readonly var activeBufferHeader = ref MemoryMarshal.AsRef<TelemetryBufferHeader>(
            TelemetryDataSpans.TelemetryBufferHeader(dataBufferIndex, _dataAccessor.Span));

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
