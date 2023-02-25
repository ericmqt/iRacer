using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacer.Tools.TelemetryVariables.Commands;
internal class DumpCommandOptions
{
    public DumpCommandOptions()
    {
        OutputFileOrDirectory = new DirectoryInfo(Environment.CurrentDirectory);
    }

    public FileSystemInfo OutputFileOrDirectory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the application should wait for the simulator to start instead of exiting immediately.
    /// </summary>
    public bool WaitForConnection { get; set; } = false;
}
