using System.Collections.Generic;

namespace Horizon
{
    interface IInternalCaller : ICaller
    {
        IReadOnlyList<SimpleParameterInfo> ParameterTypes { get; }
        bool IsStatic { get; }
    }
}