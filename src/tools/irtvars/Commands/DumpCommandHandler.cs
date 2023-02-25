using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO;

namespace iRacer.Tools.TelemetryVariables.Commands;
internal class DumpCommandHandler
{
    public DumpCommandHandler(DumpCommandOptions options, IConsole console)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public IConsole Console { get; }
    public DumpCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var connection = new SimulatorDataConnection();

        if (!await OpenConnectionAsync(connection, TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false))
        {
            return -1;
        }

        if (!await WaitForFirstDataReadyAsync(connection, cancellationToken).ConfigureAwait(false))
        {
            // TODO: Write output
            return -1;
        }

        using var dataAccessor = connection.CreateDataAccessor();

        while (!cancellationToken.IsCancellationRequested)
        {
            var header = dataAccessor.ReadHeader();

            if (header.Status > 0)
            {
                // Dump variables
                using var telemetryReader = new TelemetryDataReader(connection);

                var variableHeaders = telemetryReader.ReadVariableHeaders(header);

                // TODO: Write variables

                break;
            }
        }

        return 0;
    }

    private void GetOutputFileName()
    {
        if (Options.OutputFileOrDirectory is null)
        {
            // TODO
        }

        if (Options.OutputFileOrDirectory is DirectoryInfo outputDir)
        {
            throw new NotImplementedException();
        }
        else if (Options.OutputFileOrDirectory is FileInfo outputFile)
        {
            throw new NotImplementedException();
        }
    }

    private async Task<bool> OpenConnectionAsync(SimulatorDataConnection connection, TimeSpan retryInterval, CancellationToken cancellationToken = default)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (!Options.WaitForConnection)
        {
            return connection.Open();
        }

        // Attempt to open connection until successful or canceled
        while (!cancellationToken.IsCancellationRequested)
        {
            if (connection.Open())
            {
                return true;
            }

            try { await Task.Delay(retryInterval, cancellationToken).ConfigureAwait(false); }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        return false;
    }

    private async Task<bool> WaitForFirstDataReadyAsync(SimulatorDataConnection connection, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (await connection.WaitForDataReadyAsync(cancellationToken).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }
}
