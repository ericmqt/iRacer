using System.CommandLine;
using System.CommandLine.Binding;

namespace iRacer.Tools.TelemetryVariables.CommandLine;

internal class ConsoleBinder : BinderBase<IConsole>
{
    protected override IConsole GetBoundValue(BindingContext bindingContext)
    {
        return bindingContext.Console;
    }
}
