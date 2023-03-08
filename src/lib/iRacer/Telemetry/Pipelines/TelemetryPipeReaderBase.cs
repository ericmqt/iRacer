using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;

namespace iRacer.Telemetry.Pipelines;

public abstract class TelemetryPipeReaderBase
{
    protected abstract Task OnDataReceivedAsync(TelemetryDataFrameHeader header, ReadOnlySequence<byte> data);

    public async Task ReadPipeAsync(PipeReader pipeReader)
    {
        if (pipeReader is null)
        {
            throw new ArgumentNullException(nameof(pipeReader));
        }

        while (true)
        {
            var readResult = await pipeReader.ReadAsync().ConfigureAwait(false);
            var buffer = readResult.Buffer;

            if (readResult.IsCanceled)
            {
                break;
            }

            if (TryReadDataFrame(ref buffer, out var header, out var dataBuffer))
            {
                try { await OnDataReceivedAsync(header, dataBuffer).ConfigureAwait(false); }
                catch (Exception)
                {
                    // TODO: Log

                    throw;
                }
            }

            pipeReader.AdvanceTo(buffer.Start, buffer.End);

            if (readResult.IsCompleted)
            {
                break;
            }
        }

        await pipeReader.CompleteAsync();
    }

    protected static TelemetryDataFrameHeader ReadDataFrameHeader(ref ReadOnlySequence<byte> buffer)
    {
        var headerSequence = buffer.Slice(0, TelemetryDataFrameHeaderConstants.HeaderLength);

        if (headerSequence.IsSingleSegment)
        {
            return MemoryMarshal.Read<TelemetryDataFrameHeader>(headerSequence.First.Span);
        }

        // Read from temporary array
        Span<byte> headerSpan = new byte[TelemetryDataFrameHeaderConstants.HeaderLength];
        headerSequence.CopyTo(headerSpan);

        return MemoryMarshal.Read<TelemetryDataFrameHeader>(headerSpan);
    }

    protected static bool TryReadDataFrame(ref ReadOnlySequence<byte> buffer, out TelemetryDataFrameHeader header, out ReadOnlySequence<byte> dataBuffer)
    {
        header = default;
        dataBuffer = default;

        if (buffer.Length < TelemetryDataFrameHeaderConstants.HeaderLength)
        {
            return false;
        }

        header = ReadDataFrameHeader(ref buffer);
        dataBuffer = buffer.Slice(TelemetryDataFrameHeaderConstants.HeaderLength, header.DataLength);

        return true;
    }
}
