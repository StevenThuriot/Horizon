using System;
using System.Collections.Generic;

namespace Horizon
{
    static class Constants
    {
        public static readonly Type BooleanType = typeof (bool);
        public static readonly Type FloatType = typeof (float);
        public static readonly Type StringType = typeof (string);
        public static readonly Type DoubleType = typeof (double);
        public static readonly Type IntegerType = typeof (int);
        public static readonly Type ObjectType = typeof (object);
        public static readonly Type ObjectArrayType = typeof (object[]);
        public static readonly Type VoidType = typeof(void);
        public static readonly Type GenericDictionaryDefinition = typeof(IDictionary<,>);

        internal class Typed<T>
        {
            public static readonly Type OwnerType = typeof (T);
            
            private static bool? _isGenericDictionary;
            public static bool IsGenericDictionary
            {
                get
                {
                    if (_isGenericDictionary.HasValue)
                        return _isGenericDictionary.Value;

                    var result = OwnerType.GetInterface(GenericDictionaryDefinition.Name) != null;
                    _isGenericDictionary = result;

                    return result;
                }
            }
        }
    }
}