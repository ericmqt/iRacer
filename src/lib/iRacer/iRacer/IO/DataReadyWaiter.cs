using DotNext.Threading;

namespace iRacer.IO;

/// <summary>
/// Provides coordinated synchronous and Task-based asynchronous wait functionality for the data-ready event.
/// </summary>
internal sealed class DataReadyWaiter : IDisposable
{
    private readonly AsyncManualResetEvent _asyncSignal;
    private bool _isDisposed;
    private readonly ManualResetEvent _syncSignal;

    internal DataReadyWaiter()
    {
        _asyncSignal = new AsyncManualResetEvent(false);
        _syncSignal = new ManualResetEvent(false);
    }

    public void Reset()
    {
        _syncSignal.Reset();
        _asyncSignal.Reset();
    }

    public void Set()
    {
        _syncSignal.Set();
        _asyncSignal.Set();
    }

    public void Set(bool autoReset)
    {
        _syncSignal.Set();
        _asyncSignal.Set();

        if (autoReset)
        {
            _syncSignal.Reset();
            _asyncSignal.Reset();
        }
    }

    public bool Wait(CancellationToken cancellationToken)
    {
        return _syncSignal.Wait(cancellationToken);
    }

    public async ValueTask<bool> WaitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _asyncSignal.WaitAsync(cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _syncSignal.Dispose();
                _asyncSignal.Dispose();
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
}
