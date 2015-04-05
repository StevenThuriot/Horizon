using System.Collections.Generic;

namespace Horizon
{
    partial interface ICaller
    {
        string Name { get; }
        object Call(IEnumerable<dynamic> values);
    }
}