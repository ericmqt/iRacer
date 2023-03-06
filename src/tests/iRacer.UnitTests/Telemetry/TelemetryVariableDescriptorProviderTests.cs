using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.Telemetry;

namespace iRacer.UnitTests.Telemetry;
public class TelemetryVariableDescriptorProviderTests
{
    [Fact]
    public void Count_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variable1 = builder.Register<int>("Test1");
        var variable2 = builder.Register<int>("Test2");

        var provider = builder.Build();

        Assert.Equal(2, provider.Count);
    }

    [Fact]
    public void Indexer_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variable1 = builder.Register<int>("Test1");
        var variable2 = builder.Register<int>("Test2");

        var provider = builder.Build();

        Assert.Equal(variable1, provider[variable1.DescriptorIndex]);
        Assert.Equal(variable2, provider[variable2.DescriptorIndex]);
    }
}
