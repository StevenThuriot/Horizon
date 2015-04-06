using System.Collections.Generic;

namespace Horizon
{
    partial interface IEventCaller : ICaller
    {
        void Add(IEnumerable<dynamic> values);
        void Remove(IEnumerable<dynamic> values);
    }
}