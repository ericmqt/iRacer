using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace iRacer.IO;

internal static class DataReadyEvent
{
    internal const string EventName = "Local\\IRSDKDataValidEvent";

    internal static SafeWaitHandle CreateSafeWaitHandle()
    {
        return PInvoke.CreateEventAsSafeWaitHandle(null, true, false, EventName);
    }

    internal static bool TryCreateSafeWaitHandle([NotNullWhen(true)] out SafeWaitHandle? waitHandle)
    {
        waitHandle = PInvoke.CreateEventAsSafeWaitHandle(null, true, false, EventName);

        if (waitHandle.IsInvalid || waitHandle.IsClosed)
        {
            waitHandle = null;
            return false;
        }

        return true;
    }
}