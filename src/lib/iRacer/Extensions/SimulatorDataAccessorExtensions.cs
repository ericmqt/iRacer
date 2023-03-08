using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using iRacer.IO.Primitives;

namespace iRacer.IO;
public static class SimulatorDataAccessorExtensions
{
    public static DataFileHeaderReader CreateHeaderReader(this ISimulatorDataAccessor dataAccessor)
    {
        if (dataAccessor is null)
        {
            throw new ArgumentNullException(nameof(dataAccessor));
        }

        return new DataFileHeaderReader(dataAccessor.Span[..DataFileConstants.HeaderLength]);
    }

    public static DataFileHeader ReadHeader(this ISimulatorDataAccessor dataAccessor)
    {
        if (dataAccessor is null)
        {
            throw new ArgumentNullException(nameof(dataAccessor));
        }

        return MemoryMarshal.Read<DataFileHeader>(dataAccessor.Span[..DataFileConstants.HeaderLength]);
    }
}
