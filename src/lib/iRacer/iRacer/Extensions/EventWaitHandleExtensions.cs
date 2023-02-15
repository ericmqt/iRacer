using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacer;
internal static class EventWaitHandleExtensions
{
    internal static bool Wait(this EventWaitHandle waitHandle, CancellationToken cancellationToken)
    {
        if (waitHandle is null)
        {
            throw new ArgumentNullException(nameof(waitHandle));
        }

        if (cancellationToken == default)
        {
            throw new ArgumentException($"'{nameof(cancellationToken)}' cannot be a default value.", nameof(cancellationToken));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var waitHandles = new[] { waitHandle, cancellationToken.WaitHandle };

        return WaitHandle.WaitAny(waitHandles) == 0;
    }
}