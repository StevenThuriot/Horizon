#region License

//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

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
            public static readonly IReadOnlyCollection<Type> ArgTypes = new[] {OwnerType, ObjectArrayType};

            private static bool? _isGenericDictionary;
            public static bool IsGenericDictionary
            {
                get
                {
                    if (_isGenericDictionary.HasValue)
                        return _isGenericDictionary.Value;

                    if (OwnerType.GetInterface(GenericDictionaryDefinition.Name) == null)
                    {
                        _isGenericDictionary = false;
                        return false;
                    }

                    _isGenericDictionary = true;
                    return true;
                }
            }
        }
    }
}