using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using iRacer.Tools.TelemetryVariables.CommandLine;
using iRacer.Tools.TelemetryVariables.Commands;

namespace iRacer.Tools.TelemetryVariables;

internal class Program
{
    static Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("irtvars");

        // Add subcommands
        rootCommand.AddCommand(CreateDumpCommand());

        // Build and run
        var builder = new CommandLineBuilder(rootCommand)
            .UseDefaults();

        var parser = builder.Build();

        return parser.InvokeAsync(args);
    }

    private static Command CreateDumpCommand()
    {
        var cmd = new Command("dump");

        // Arguments
        var outputArg = new Argument<FileSystemInfo>("output-file-or-dir", "Output filename or directory. Defaults to current directory.")
        {
            Arity = new ArgumentArity(0, 1)
        };

        outputArg.SetDefaultValueFactory(() => new DirectoryInfo(Environment.CurrentDirectory));

        cmd.AddArgument(outputArg);

        // Options
        var waitForConnectionOption = new Option<bool>(
            new[] { "--wait", "-w" },
            "Wait for the simulator instead of exiting if the simulator is not running.");

        cmd.AddOption(waitForConnectionOption);

        // Handler
        cmd.SetHandler(
            (options, console, cancellationToken) =>
            {
                var handler = new DumpCommandHandler(options, console);

                return handler.ExecuteAsync(cancellationToken);
            },
            new DumpCommandOptionsBinder(outputArg, waitForConnectionOption),
            new ConsoleBinder(),
            new CancellationTokenBinder());

        return cmd;
    }
}
