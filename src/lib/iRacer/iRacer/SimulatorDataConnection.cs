using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO;

namespace iRacer;
public sealed class SimulatorDataConnection : IDisposable
{
    private int _connectionId;
    private SimulatorDataFile? _dataFile;
    private DataReadyEventMonitor? _dataReadyMonitor;
    private readonly DataReadyWaiter _dataReadyWaiter;
    private bool _isDisposed;

    public SimulatorDataConnection()
    {
        _dataReadyWaiter = new DataReadyWaiter();
    }

    /// <summary>
    /// Gets a value indicating the number of times the connection has been opened, or -1 if the connection is closed.
    /// </summary>
    public int ConnectionId
    {
        get { return IsOpen ? _connectionId : -1; }
    }

    public bool IsActive
    {
        get
        {
            if (_dataReadyMonitor is null)
            {
                return false;
            }

            return _dataReadyMonitor.IsActive;
        }
    }

    [MemberNotNullWhen(true, nameof(_dataFile))]
    [MemberNotNullWhen(true, nameof(_dataReadyMonitor))]
    public bool IsOpen { get; private set; }

    public void Close()
    {
        IsOpen = false;

        _dataReadyMonitor?.Dispose();
        _dataReadyMonitor = null;

        _dataFile?.Dispose();
        _dataFile = null;
    }

    public ISimulatorDataAccessor CreateDataAccessor()
    {
        if (!IsOpen)
        {
            // TODO: Better exception?
            throw new InvalidOperationException("The connection is closed.");
        }

        return _dataFile.CreateDataAccessor();
    }

    public bool Open()
    {
        if (IsOpen)
        {
            return true;
        }

        // Acquire memory-mapped file first. We will always be able to create the data-ready event, so being unable to acquire the
        // memory-mapped file is a better first indicator that the simulator is not running.

        if (!SimulatorDataFile.TryOpen(out var dataFile))
        {
            return false;
        }

        // Get the data-ready event. This should always succeed (if I am reading the Win32 docs correctly), but be careful just in case.
        if (!DataReadyEvent.TryCreateSafeWaitHandle(out var hDataReadyEvent))
        {
            // Clean up the data file we've already opened.
            dataFile.Dispose();

            return false;
        }

        _dataFile = dataFile;

        _connectionId++;
        IsOpen = true;

        // Start the data-ready event monitor
        _dataReadyMonitor = new DataReadyEventMonitor(
            hDataReadyEvent,
            _dataReadyWaiter,
            _dataFile.CreateDataAccessor(),
            OnDataSessionInitialized);

        return true;
    }

    public bool WaitForDataReady()
    {
        if (!IsOpen)
        {
            return false;
        }

        return _dataReadyWaiter.Wait(_dataReadyMonitor.CancellationToken);
    }

    public bool WaitForDataReady(CancellationToken cancellationToken)
    {
        if (!IsOpen)
        {
            return false;
        }

        using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_dataReadyMonitor.CancellationToken, cancellationToken);

        return _dataReadyWaiter.Wait(linkedCancellationSource.Token);
    }

    public async ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken = default)
    {
        if (!IsOpen)
        {
            return false;
        }

        // Combine our event monitor thread cancellation token with the provided one to ensure waiters are returned false as soon as
        // the the connection is closed.

        using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_dataReadyMonitor.CancellationToken, cancellationToken);

        return await _dataReadyWaiter.WaitAsync(linkedCancellationSource.Token).ConfigureAwait(false);
    }

    private void OnDataSessionInitialized()
    {
        // Placeholder for initialization that may occur on the first data-ready signal received, e.g. creating the telemetry variable table.
    }

    #region IDisposable Implementation

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _dataReadyMonitor?.Dispose();
                _dataReadyMonitor = null;

                _dataFile?.Dispose();
                _dataFile = null;

                _dataReadyWaiter.Dispose();
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
