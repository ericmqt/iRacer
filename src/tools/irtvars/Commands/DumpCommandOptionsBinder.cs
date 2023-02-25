using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacer.Tools.TelemetryVariables.Commands;
internal class DumpCommandOptionsBinder : BinderBase<DumpCommandOptions>
{
    private readonly Argument<FileSystemInfo> _outputFileOrDirectoryArgument;
    private readonly Option<bool> _waitForConnectionOption;

    public DumpCommandOptionsBinder(Argument<FileSystemInfo> outputFileOrDirectoryArgument, Option<bool> waitForConnectionOption)
    {
        _outputFileOrDirectoryArgument = outputFileOrDirectoryArgument ?? throw new ArgumentNullException(nameof(outputFileOrDirectoryArgument));
        _waitForConnectionOption = waitForConnectionOption ?? throw new ArgumentNullException(nameof(waitForConnectionOption));
    }

    protected override DumpCommandOptions GetBoundValue(BindingContext bindingContext)
    {
        var outputFileOrDirectory = bindingContext.ParseResult.GetValueForArgument(_outputFileOrDirectoryArgument);
        var waitForConnection = bindingContext.ParseResult.GetValueForOption(_waitForConnectionOption);

        return new DumpCommandOptions()
        {
            OutputFileOrDirectory = outputFileOrDirectory,
            WaitForConnection = waitForConnection
        };
    }
}
