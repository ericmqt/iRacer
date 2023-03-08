using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.MemoryMappedFiles;
using DotNext.IO.MemoryMappedFiles;

namespace iRacer.IO;
internal sealed class SimulatorDataFile : IDisposable
{
    public const string MemoryMappedFileName = "Local\\IRSDKMemMapFileName";

    private readonly List<ISimulatorDataAccessor> _accessors;
    private readonly MemoryMappedFile _dataFile;
    private bool _isDisposed;

    internal SimulatorDataFile(MemoryMappedFile dataFile)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));

        _accessors = new List<ISimulatorDataAccessor>();
    }

    public static bool TryOpen([NotNullWhen(true)] out SimulatorDataFile? simulatorDataFile)
    {
        try
        {
            var dataFile = MemoryMappedFile.OpenExisting(MemoryMappedFileName);

            simulatorDataFile = new SimulatorDataFile(dataFile);
            return true;
        }
        catch (FileNotFoundException)
        {
            simulatorDataFile = null;
            return false;
        }
    }

    public ISimulatorDataAccessor CreateDataAccessor()
    {
        var accessor = new SimulatorDataAccessor(_dataFile.CreateDirectAccessor(0, 0, MemoryMappedFileAccess.Read));

        // Keep reference to all accessors so we can dispose them
        _accessors.Add(accessor);

        return accessor;
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed objects
                while (_accessors.Count > 0)
                {
                    _accessors[0].Dispose();

                    _accessors.RemoveAt(0);
                }

                Debug.Assert(_accessors.Count == 0);

                _dataFile.Dispose();
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
