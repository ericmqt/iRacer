using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.Telemetry;

namespace iRacer.UnitTests.Telemetry;
public class TelemetryVariableDescriptorProviderBuilderTests
{
    [Fact]
    public void Build_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var scalarVariable1Name = "Test1";
        var scalarVariable1 = builder.Register<int>(scalarVariable1Name);

        var scalarVariable2Name = "Test2";
        var scalarVariable2 = builder.Register<uint>(scalarVariable2Name);

        var arrayVariable1Name = "TestArray1";
        var arrayVariable1Length = 42;
        var arrayVariable1 = builder.RegisterArray<short>(arrayVariable1Name, arrayVariable1Length);

        var arrayVariable2Name = "TestArray2";
        var arrayVariable2Length = 3;
        var arrayVariable2 = builder.RegisterArray<int>(arrayVariable2Name, arrayVariable2Length);

        var provider = builder.Build();

        var afterBuildVariable = builder.Register<int>("Test3");

        Assert.NotNull(provider);
        Assert.Contains(scalarVariable1, provider);
        Assert.Contains(scalarVariable2, provider);
        Assert.Contains(arrayVariable1, provider);
        Assert.Contains(arrayVariable2, provider);
        Assert.DoesNotContain(afterBuildVariable, provider);
    }

    [Fact]
    public void Build_Empty_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var provider = builder.Build();

        Assert.NotNull(provider);
        Assert.Equal(0, provider.Count);
    }

    [Fact]
    public void IsRegistered_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variableName = "Test";

        // Scalar variable
        Assert.False(builder.IsRegistered(variableName));

        var descriptor = builder.Register<int>(variableName);

        Assert.True(builder.IsRegistered(variableName));

        // Array variable
        var arrayVariableName = "ArrayTest";
        var arrayLength = 2;

        Assert.False(builder.IsRegistered(arrayVariableName));

        var arrayDescriptor = builder.RegisterArray<int>(arrayVariableName, arrayLength);

        Assert.True(builder.IsRegistered(arrayVariableName));
    }

    [Fact]
    public void Register_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variableName = "Test";
        var descriptor = builder.Register<int>(variableName);

        Assert.NotNull(descriptor);
        Assert.Equal(variableName, descriptor.Name);
        Assert.Equal(typeof(int), descriptor.ValueType);
    }

    [Fact]
    public void Register_ThrowsOnNullOrEmptyVariableName_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        Assert.Throws<ArgumentException>(() => builder.Register<int>(null!));
        Assert.Throws<ArgumentException>(() => builder.Register<int>(string.Empty));
    }

    [Fact]
    public void Register_ThrowsOnDuplicateVariableName_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variableName = "Test";
        var descriptor = builder.Register<int>(variableName);

        Assert.NotNull(descriptor);

        Assert.Throws<ArgumentException>(() => builder.Register<int>(variableName));
    }

    [Fact]
    public void RegisterArray_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variableName = "Test";
        var arrayLength = 6;

        var descriptor = builder.RegisterArray<int>(variableName, arrayLength);

        Assert.NotNull(descriptor);
        Assert.Equal(variableName, descriptor.Name);
        Assert.Equal(arrayLength, descriptor.ArrayLength);
        Assert.Equal(typeof(int), descriptor.ValueType);
    }

    [Fact]
    public void RegisterArray_ThrowsOnDuplicateVariableName_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        var variableName = "Test";
        var arrayLength = 6;
        var descriptor = builder.RegisterArray<int>(variableName, arrayLength);

        Assert.NotNull(descriptor);

        Assert.Throws<ArgumentException>(() => builder.RegisterArray<int>(variableName, arrayLength));
    }

    [Fact]
    public void RegisterArray_ThrowsOnInvalidArrayLength_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() => builder.RegisterArray<int>("Test", -1));
    }

    [Fact]
    public void RegisterArray_ThrowsOnNullOrEmptyVariableName_Test()
    {
        var builder = new TelemetryVariableDescriptorProviderBuilder();

        Assert.Throws<ArgumentException>(() => builder.RegisterArray<int>(null!, 6));
        Assert.Throws<ArgumentException>(() => builder.RegisterArray<int>(string.Empty, 6));
    }
}
