using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacer.UnitTests.IO;
internal static class MemoryMappedFileFactory
{
    internal static MemoryMappedFile CreateWithRandomName(long capacity = 33_792)
    {
        return MemoryMappedFile.CreateNew(Path.GetRandomFileName(), capacity);
    }

}
