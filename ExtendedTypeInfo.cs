using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    static partial class TypeInfo<T>
    {
        public static class Extended
        {
            public static IEnumerable<IMethodCaller> Methods
            {
                get { return _methods.SelectMany(callers => callers); }
            }

            public static IEnumerable<ICaller> Constructors
            {
                get { return _constructors.AsReadOnly(); }
            }

            public static IEnumerable<IEventCaller> Events
            {
                get { return _events.Values; }
            }

            public static IEnumerable<IPropertyCaller<T>> Properties
            {
                get { return _properties.Values; }
            }

            public static IEnumerable<IMemberCaller<T>> Fields
            {
                get { return _fields.Values; }
            }

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