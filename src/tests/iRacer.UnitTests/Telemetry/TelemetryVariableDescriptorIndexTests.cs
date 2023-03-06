using iRacer.Telemetry;

namespace iRacer.UnitTests.Telemetry;

public class TelemetryVariableDescriptorIndexTests
{
    [Fact]
    public void Constructor_Test()
    {
        var index = 9;
        var descriptorIndex = new TelemetryVariableDescriptorIndex(index);

        Assert.Equal(index, descriptorIndex);
    }

    [Fact]
    public void Comparison_Test()
    {
        var equalIndex1 = new TelemetryVariableDescriptorIndex(1);
        var equalIndex2 = new TelemetryVariableDescriptorIndex(1);

        Assert.True(equalIndex1.CompareTo(equalIndex2) == 0);
        Assert.False(equalIndex1 < equalIndex2);
        Assert.True(equalIndex1 <= equalIndex2);
        Assert.False(equalIndex1 > equalIndex2);
        Assert.True(equalIndex1 >= equalIndex2);

        var greaterIndex = new TelemetryVariableDescriptorIndex(50);
        var lesserIndex = new TelemetryVariableDescriptorIndex(25);

        Assert.True(greaterIndex.CompareTo(lesserIndex) > 0);
        Assert.True(lesserIndex.CompareTo(greaterIndex) < 0);

        Assert.True(greaterIndex > lesserIndex);
        Assert.True(greaterIndex >= lesserIndex);
        Assert.False(greaterIndex < lesserIndex);
        Assert.False(greaterIndex <= lesserIndex);

        Assert.False(lesserIndex > greaterIndex);
        Assert.False(lesserIndex >= greaterIndex);
        Assert.True(lesserIndex < greaterIndex);
        Assert.True(lesserIndex <= greaterIndex);
    }

    [Fact]
    public void Equals_Test()
    {
        var index1 = 0;
        var index2 = 2;

        // Test two equivalent values
        var descriptorIndex1 = new TelemetryVariableDescriptorIndex(index1);
        var descriptorIndex2 = new TelemetryVariableDescriptorIndex(index1);

        Assert.True(descriptorIndex1.Equals(descriptorIndex2));
        Assert.True(descriptorIndex1.Equals((object?)descriptorIndex2));
        Assert.True(descriptorIndex1 == descriptorIndex2);
        Assert.False(descriptorIndex1 != descriptorIndex2);

        // Test two different values
        descriptorIndex2 = new TelemetryVariableDescriptorIndex(index2);

        Assert.False(descriptorIndex1.Equals(descriptorIndex2));
        Assert.False(descriptorIndex1.Equals((object?)descriptorIndex2));
        Assert.False(descriptorIndex1 == descriptorIndex2);
        Assert.True(descriptorIndex1 != descriptorIndex2);
    }
}
