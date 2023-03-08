using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO;

namespace iRacer.UnitTests.IO;
public class SimulatorDataFileTests
{
    [Fact]
    public void CreateDataAccessorTest()
    {
        using var mmFile = MemoryMappedFileFactory.CreateWithRandomName();
        using var dataFile = new SimulatorDataFile(mmFile);

        var accessor = dataFile.CreateDataAccessor();

        Assert.NotNull(accessor);
    }

    [Fact]
    public void DisposeCreatedDataAccessorsTest()
    {
        using var mmFile = MemoryMappedFileFactory.CreateWithRandomName();
        using var dataFile = new SimulatorDataFile(mmFile);

        var accessor = dataFile.CreateDataAccessor();

        Assert.True(accessor.CanRead);

        dataFile.Dispose();

        Assert.False(accessor.CanRead);
    }
}
