using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using iRacer.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace iRacer.Telemetry.Pipelines;
public class TelemetryPipeWriter
{
    private readonly TelemetryPipeWriterLogger _logger;

    public TelemetryPipeWriter(SimulatorDataConnection connection)
        : this(connection, NullLogger<TelemetryPipeWriter>.Instance)
    {

    }

    public TelemetryPipeWriter(SimulatorDataConnection connection, ILogger<TelemetryPipeWriter> logger)
    {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _logger = new TelemetryPipeWriterLogger(logger);
    }

    public SimulatorDataConnection Connection { get; }

    [SuppressMessage(
        "Reliability",
        "CA2016:Forward the 'CancellationToken' parameter to methods",
        Justification = "Method cooperatively cancels to avoid throwing OperationCanceledException.")]
    public async Task WriteToPipeAsync(PipeWriter pipeWriter, CancellationToken cancellationToken = default)
    {
        if (pipeWriter is null)
        {
            throw new ArgumentNullException(nameof(pipeWriter));
        }

        _logger.PipeWriterStarted();

        using var reader = new SimulatorDataReader(Connection);

        // Wait for data, otherwise complete the pipe if canceled
        while (await Connection.WaitForDataReadyAsync(cancellationToken).ConfigureAwait(false))
        {
            // Get the length in bytes of the next data line
            var dataLength = reader.ReadTelemetryBufferLength();

            // Initialize the frame header
            var frameHeader = new TelemetryDataFrameHeader()
            {
                Status = reader.ReadSimulatorStatus(),
                DataLength = dataLength,
                VariableCount = reader.ReadTelemetryVariableCount()
            };

            // Get some memory that is at least as large as our data line plus the header
            var memory = pipeWriter.GetMemory(TelemetryDataFrameHeaderConstants.HeaderLength + dataLength);

            try
            {
                var telemetryDataMemory = memory.Slice(TelemetryDataFrameHeaderConstants.HeaderLength, dataLength);

                if (reader.TryReceiveTelemetryData(telemetryDataMemory, out var bytesRead, out var tickCount))
                {
                    // Set the tick count
                    frameHeader.Ticks = tickCount;

                    // Write frame header
                    MemoryMarshal.Write(memory[..TelemetryDataFrameHeaderConstants.HeaderLength].Span, ref frameHeader);

                    pipeWriter.Advance(TelemetryDataFrameHeaderConstants.HeaderLength + bytesRead);
                }
                else
                {
                    // Data line was swapped out from under us
                    _logger.ReceiveTelemetryDataFailed();
                }
            }
            catch (Exception ex)
            {
                _logger.ReadDataException(ex);

                break;
            }

            // Make data available to PipeReader
            var flushResult = await pipeWriter.FlushAsync().ConfigureAwait(false);

            if (flushResult.IsCompleted)
            {
                break;
            }
        }

        await pipeWriter.CompleteAsync();
        _logger.PipeWriterCompleted();
    }
}
