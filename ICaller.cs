using System.Collections.Generic;

namespace Horizon
{
    partial interface ICaller
    {
        object Call(IEnumerable<dynamic> values);
    }
}