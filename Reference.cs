using System.Runtime;
using System.Runtime.CompilerServices;

namespace Horizon
{
    static class Reference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static bool IsNull(object value) => value == null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static bool IsNotNull(object value) => value != null;
    }
}