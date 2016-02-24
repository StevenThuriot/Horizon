using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    static partial class Info<T>
    {
        public static class Extended
        {
            public static IEnumerable<IMethodCaller> Methods => container.Methods.SelectMany(callers => callers);

            public static IEnumerable<IConstructorCaller> Constructors => container.Constructors;

            public static IConstructorCaller DefaultConstructor => Info.Extended.ResolveSpecificCaller(Constructors, Type.EmptyTypes);

            public static IEnumerable<IEventCaller> Events => container.Events.Values;

            public static IEnumerable<IPropertyCaller<T>> Properties => container.Properties.Values;

            public static IEnumerable<IMemberCaller<T>> Fields => container.Fields.Values;

            public static IEnumerable<IMemberCaller<T>> Members
            {
                get
                {
                    foreach (var caller in Properties)
                        yield return caller;

                    foreach (var caller in Fields)
                        yield return caller;
                }
            }
        }
    }
}