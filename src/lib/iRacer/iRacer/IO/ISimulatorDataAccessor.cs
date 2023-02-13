namespace iRacer.IO;

/// <summary>
/// Provides direct access to the simulator data file.
/// </summary>
public interface ISimulatorDataAccessor : IDisposable
{
    /// <summary>
    /// Gets value indicating if <see cref="Span"/> may be safely accessed for reading.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets the length of the simulator data file.
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Gets a read-only span of bytes representing the simulator data file.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when <see cref="ISimulatorDataAccessor"/> has been disposed.</exception>
    ReadOnlySpan<byte> Span { get; }
}
