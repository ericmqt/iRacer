using Microsoft.Extensions.Logging;

namespace iRacer.Telemetry.Pipelines;

internal sealed partial class TelemetryPipeWriterLogger : ILogger<TelemetryPipeWriter>
{
    private readonly ILogger<TelemetryPipeWriter> _logger;

    public TelemetryPipeWriterLogger(ILogger<TelemetryPipeWriter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [LoggerMessage(
        EventId = 1,
        EventName = nameof(PipeWriterStarted),
        Level = LogLevel.Debug,
        Message = "Begin writing telemetry data to pipe.")]
    public partial void PipeWriterStarted();

    [LoggerMessage(
        EventId = 2,
        EventName = nameof(PipeWriterCompleted),
        Level = LogLevel.Information,
        Message = "Telemetry data pipe writer completed.")]
    public partial void PipeWriterCompleted();

    [LoggerMessage(
        EventId = 3,
        EventName = nameof(ReceiveTelemetryDataFailed),
        Level = LogLevel.Warning,
        Message = "Failed to read telemetry data line before it was overwritten by the simulator.")]
    public partial void ReceiveTelemetryDataFailed();

    [LoggerMessage(
        EventId = 4,
        EventName = nameof(ReadDataException),
        Level = LogLevel.Error,
        Message = "Unexpected exception thrown while reading telemetry data.")]
    public partial void ReadDataException(Exception ex);

    #region ILogger Implementation

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope<TState>(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log<TState>(logLevel, eventId, state, exception, formatter);
    }

    #endregion ILogger Implementation
}
