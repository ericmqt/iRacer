using Microsoft.Win32.SafeHandles;
using Windows.Win32.Foundation;
using Windows.Win32.Security;

namespace Windows.Win32;
internal static partial class PInvoke
{
    internal static unsafe SafeWaitHandle CreateEventAsSafeWaitHandle(SECURITY_ATTRIBUTES? lpEventAttributes, BOOL bManualReset, BOOL bInitialState, string lpName)
    {
        fixed (char* lpNameLocal = lpName)
        {
            SECURITY_ATTRIBUTES lpEventAttributesLocal =
                lpEventAttributes.HasValue
                ? lpEventAttributes.Value
                : default;

            var handle = CreateEvent(
                lpEventAttributes.HasValue ? &lpEventAttributesLocal : null,
                bManualReset,
                bInitialState,
                lpNameLocal);

            return new SafeWaitHandle(handle, ownsHandle: true);
        }
    }
}
