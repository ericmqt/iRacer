using System.Runtime.InteropServices;
using iRacer.IO.Primitives;
using Microsoft.Win32.SafeHandles;

namespace iRacer.IO;
internal sealed class DataReadyEventMonitor : IDisposable
{
    private int _activityTimeoutMs;
    private readonly AutoResetEvent _dataReadyEvent;
    private readonly SafeWaitHandle _hDataReadyEvent;
    private readonly DataReadyWaiter _dataReadyWaiter;
    private readonly ISimulatorDataAccessor _dataAccessor;
    private bool _isDisposed;
    private readonly Action? _onActivated;
    private readonly Thread _thread;
    private readonly CancellationTokenSource _threadCancellationTokenSource;

    public DataReadyEventMonitor(
        SafeWaitHandle hDataReadyEvent,
        DataReadyWaiter dataReadyWaiter,
        ISimulatorDataAccessor dataAccessor,
        Action? onActivated = null)
    {
        _hDataReadyEvent = hDataReadyEvent ?? throw new ArgumentNullException(nameof(hDataReadyEvent));
        _dataReadyWaiter = dataReadyWaiter ?? throw new ArgumentNullException(nameof(dataReadyWaiter));
        _dataAccessor = dataAccessor ?? throw new ArgumentNullException(nameof(dataAccessor));
        _onActivated = onActivated;

        ActivityTimeout = TimeSpan.FromSeconds(5);

        _dataReadyEvent = new AutoResetEvent(initialState: false) { SafeWaitHandle = _hDataReadyEvent };
        _threadCancellationTokenSource = new CancellationTokenSource();

        CancellationToken = _threadCancellationTokenSource.Token;

        _thread = new Thread(new ThreadStart(RunThread));
        _thread.Start();
    }

    /// <summary>
    /// Gets or sets the amount of time to elapse without receiving a signal before the simulator signal is considered timed out.
    /// </summary>
    public TimeSpan ActivityTimeout
    {
        get => TimeSpan.FromMilliseconds(_activityTimeoutMs);
        set
        {
            ValidateActivityTimeout(value);
            Interlocked.Exchange(ref _activityTimeoutMs, (int)value.TotalMilliseconds);
        }
    }

    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets a value indicating that at least one data-ready signal has been received and the simulator is not timed out.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets a value indicating if the data-ready signal has been received within the timeout period specified by <see cref="ActivityTimeout"/>.
    /// </summary>
    public bool IsTimedOut { get; private set; }

    /// <summary>
    /// Signals the monitoring thread for cancellation and stops the monitor.
    /// </summary>
    public void Cancel()
    {
        if (!_threadCancellationTokenSource.IsCancellationRequested)
        {
            _threadCancellationTokenSource.Cancel();
        }
    }

    private int ReadSimulatorStatus()
    {
        return MemoryMarshal.Read<int>(DataFileHeaderSpans.Status(_dataAccessor.Span));
    }

    private void RunThread()
    {
        IsActive = false;
        IsTimedOut = false;

        // Create a wait handle array including our event and the cancellation token
        var waitHandles = new[] { _dataReadyEvent!, CancellationToken.WaitHandle };

        // Loop until the simulator is activated so we can fire the activation event.
        // Because the activation event is only ever fired once, we can save a few ops by breaking this out into its own loop, rather than
        // check every single iteration if we need to fire the activation event.

        while (!CancellationToken.IsCancellationRequested)
        {
            var waitIndex = WaitHandle.WaitAny(waitHandles, _activityTimeoutMs);

            if (waitIndex == 0)
            {
                if (ReadSimulatorStatus() == 1)
                {
                    IsActive = true;

                    _onActivated?.Invoke();

                    break;
                }
            }
            else if (waitIndex == 1)
            {
                // Canceled
                break;
            }

            // Activity timeout does not matter here, because we won't be active to begin with, so that case can be omitted.
        }

        // Monitor the data-ready event
        while (!CancellationToken.IsCancellationRequested)
        {
            // Wait for data-ready signal, cancellation, or timeout
            var waitIndex = WaitHandle.WaitAny(waitHandles, _activityTimeoutMs);

            // Toggle IsActive based on current header value
            IsActive = ReadSimulatorStatus() == 1;

            if (waitIndex == 0)
            {
                IsTimedOut = false;

                // Signal to waiting threads
                _dataReadyWaiter.Set(autoReset: true);
            }
            else if (waitIndex == 1)
            {
                // Returned wait handle index was from the cancellation token
                break;
            }
            else if (waitIndex == WaitHandle.WaitTimeout)
            {
                IsTimedOut = true;
            }
        }
    }

    private static void ValidateActivityTimeout(TimeSpan value)
    {
        if (value == Timeout.InfiniteTimeSpan)
        {
            throw new InvalidOperationException($"Value cannot be an infinite {nameof(TimeSpan)}.");
        }

        if (value < TimeSpan.Zero)
        {
            throw new InvalidOperationException($"Value cannot be a {nameof(TimeSpan)} less than the constant {nameof(TimeSpan)}.{nameof(TimeSpan.Zero)}.");
        }
    }

    #region IDisposable Implementation

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Signal the cancellation token for the thread
                _threadCancellationTokenSource.Cancel();

                // Wait for the thread to stop
                _thread.Join();

                // Clean up
                _threadCancellationTokenSource.Dispose();
                _dataReadyEvent.Dispose();
                _hDataReadyEvent.Dispose();
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
