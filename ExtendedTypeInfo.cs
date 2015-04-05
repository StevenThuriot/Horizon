using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    static partial class TypeInfo<T>
    {
        public static class Extended
        {
            public static IEnumerable<MethodCaller> Methods
            {
                get { return _methods.SelectMany(callers => callers); }
            }

            public static IEnumerable<ConstructorCaller> Constructors
            {
                get { return _constructors.AsReadOnly(); }
            }

            public static IEnumerable<KeyValuePair<string, Lazy<Func<T, object>>>> Getters
            {
                get
                {
                    foreach (var property in _getProperties)
                    {
                        yield return property;
                    }

                    foreach (var field in _getFields)
                    {
                        yield return field;
                    }
                }
            }

            public static IEnumerable<KeyValuePair<string, Lazy<Action<T, object>>>> Setters
            {
                get
                {
                    foreach (var property in _setProperties)
                    {
                        yield return property;
                    }

                    foreach (var field in _setFields)
                    {
                        yield return field;
                    }
                }
            }
        }
    }
}