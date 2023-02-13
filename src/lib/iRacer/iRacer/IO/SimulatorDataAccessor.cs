using System.Diagnostics.CodeAnalysis;
using DotNext.IO.MemoryMappedFiles;

namespace iRacer.IO;

/// <summary>
/// Provides direct access to the memory-mapped simulator data file.
/// </summary>
internal sealed class SimulatorDataAccessor : ISimulatorDataAccessor
{
    [SuppressMessage("Style", "IDE0044:Add readonly modifier",
        Justification = "MemoryMappedDirectAccessor.Dispose sets its value to default(MemoryMappedDirectAccessor), readonly modifier prevents this")]
    private MemoryMappedDirectAccessor _data;
    private bool _isDisposed;

    public SimulatorDataAccessor(MemoryMappedDirectAccessor dataAccessor)
    {
        _data = dataAccessor;
    }

    ~SimulatorDataAccessor()
    {
        // TODO: Review if a finalizer is necessary here. See notes in Dispose() method below.

        Dispose();
    }

    /// <inheritdoc />
    public bool CanRead => !_data.IsEmpty;

    /// <inheritdoc />
    public long Length => _data.Size;

    /// <inheritdoc />
    public ReadOnlySpan<byte> Span
    {
        get
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(SimulatorDataAccessor));
            }

            return _data.Bytes;
        }
    }

    public void Dispose()
    {
        // NOTE: This is a deviation from the recommended Dispose pattern with a protected Dispose(bool disposing) method.
        //
        // MemoryMappedDirectAccessor is a value type with no finalizer, and due to the nature of the resource it encapsulates it MUST
        // be disposed. Its Dispose() method sets its value to `default` after cleaning up, so multiple calls are OK, and it won't matter
        // if our Dispose method is called from a consuming type or our finalizer. This obviates the Dispose(bool) method.
        //
        // The finalizer is included for now to guarantee that the MemoryMappedDirectAccessor is disposed. For now, SimulatorDataAccessor
        // objects can ONLY be created by the SimulatorDataFile object, which keeps references to created SimulatorDataAccessor objects and
        // disposes of them when itself is disposed, so a finalizer here is potentially unnecessary.

        if (!_isDisposed)
        {
            _data.Dispose();

            _isDisposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
