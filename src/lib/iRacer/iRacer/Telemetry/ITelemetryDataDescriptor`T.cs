using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacer.Telemetry;
public interface ITelemetryDataDescriptor<T> : ITelemetryDataDescriptor
    where T : unmanaged
{
}
