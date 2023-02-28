using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO;
using iRacer.IO.Primitives;
using iRacer.Tools.Telemetry;

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

        Console.WriteLine("Connecting to simulator");

        if (!await OpenConnectionAsync(connection, TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false))
        {
            Console.WriteLine("Unable to connect to simulator");
            return -1;
        }

        Console.WriteLine("Connected. Waiting for simulator activity");

        if (!await WaitForSimulatorStartup(connection, cancellationToken).ConfigureAwait(false))
        {
            // We were canceled, so just exit without any ceremony.

            // TODO: Use distinct exit codes

            return -1;
        }

        // Pull telemetry variables from the simulator
        Console.WriteLine("Reading telemetry variables");
        var telemetryVariables = GetTelemetryVariables(connection);

        Console.WriteLine($"Found {telemetryVariables.Count} telemetry variables");

        // Write variables to file
        var outputFileName = GetOutputFileName();

        using (var stream = File.CreateText(outputFileName))
        {
            await VariableInfoCollectionSerializer.WriteJsonAsync(telemetryVariables, stream);
        }

        Console.WriteLine($"Output: {outputFileName}");

        return 0;
    }

    private VariableInfoCollection GetTelemetryVariables(SimulatorDataConnection connection)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (!connection.IsOpen)
        {
            throw new ArgumentException($"Connection is not open.", nameof(connection));
        }

        if (!connection.IsActive)
        {
            throw new ArgumentException($"Connection is not active.", nameof(connection));
        }

        var variableCollection = new VariableInfoCollection();

        using var dataAccessor = connection.CreateDataAccessor();
        using var telemetryReader = new TelemetryDataReader(connection);

        var header = dataAccessor.ReadHeader();

        var variableHeaders = telemetryReader.ReadVariableHeaders(header);

        for (int i = 0; i < variableHeaders.Length; i++)
        {
            var varHeader = variableHeaders[i];

            var varInfo = new VariableInfo(
                VariableHeader.GetNameString(varHeader),
                (VariableValueType)varHeader.Type,
                varHeader.Count,
                VariableHeader.GetDescriptionString(varHeader),
                VariableHeader.GetUnitString(varHeader),
                varHeader.CountAsTime);

            variableCollection.TryAdd(varInfo);
        }

        return variableCollection;
    }

    private string GetOutputFileName()
    {
        if (Options.OutputFileOrDirectory is DirectoryInfo outputDir)
        {
            var name = $"TelemetryVariables_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.json";

            return Path.Combine(outputDir.FullName, name);
        }
        else if (Options.OutputFileOrDirectory is FileInfo outputFile)
        {
            return outputFile.FullName;
        }

        throw new InvalidOperationException();
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

    private async Task<bool> WaitForSimulatorStartup(SimulatorDataConnection connection, CancellationToken cancellationToken = default)
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
