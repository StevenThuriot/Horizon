using System.Runtime;
using System.Runtime.CompilerServices;

namespace Invocation
{
    static class Reference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static bool IsNull(object value)
        {
            return value == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static bool IsNotNull(object value)
        {
            return value != null;
        }
    }
}
