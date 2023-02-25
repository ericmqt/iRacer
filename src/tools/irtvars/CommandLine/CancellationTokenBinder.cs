using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace iRacer.Tools.TelemetryVariables.CommandLine;

internal class CancellationTokenBinder : BinderBase<CancellationToken>
{
    protected override CancellationToken GetBoundValue(BindingContext bindingContext)
    {
        return bindingContext.GetRequiredService<CancellationToken>();
    }
}