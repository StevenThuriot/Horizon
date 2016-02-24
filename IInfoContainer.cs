using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    interface IInfoContainer
    {
        ILookup<string, MethodCaller> Methods { get; }

        IReadOnlyList<ConstructorCaller> Constructors { get; }

        IReadOnlyDictionary<string, EventCaller> Events { get; }

        IReadOnlyDictionary<string, IPropertyCaller> Properties { get; }
        IReadOnlyList<IPropertyCaller> Indexers { get; }

        IReadOnlyDictionary<string, IMemberCaller> Fields { get; }
    }
}
