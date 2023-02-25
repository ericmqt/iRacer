using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace iRacer.Tools.TelemetryVariables.CommandLine;

internal class RequiredServiceBinder<TService> : BinderBase<TService>
    where TService : notnull
{
    protected override TService GetBoundValue(BindingContext bindingContext)
    {
        return bindingContext.GetRequiredService<TService>();
    }
}