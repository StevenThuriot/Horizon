using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    static partial class Info<T>
    {
        public static class Extended
        {
            public static IEnumerable<IMethodCaller> Methods => _methods.SelectMany(callers => callers);

            public static IEnumerable<IConstructorCaller> Constructors => _constructors.AsReadOnly();

            public static IConstructorCaller DefaultConstructor => Info.Extended.ResolveSpecificCaller(_constructors, Type.EmptyTypes);

            public static IEnumerable<IEventCaller> Events => _events.Values;

            public static IEnumerable<IPropertyCaller<T>> Properties => _properties.Values;

            public static IEnumerable<IMemberCaller<T>> Fields => _fields.Values;

            public static IEnumerable<IMemberCaller<T>> Members
            {
                get
                {
                    foreach (var caller in _properties.Values)
                        yield return caller;

                    foreach (var caller in _fields.Values)
                        yield return caller;
                }
            }
        }
    }
}