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
using System.Linq;

namespace Invocation
{
    static class GenericType
    {
        private static readonly Type[] EmptyTypes = new Type[0];

        public static bool IsGenericTypeOf(this Type type, Type genericDefinition)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition())
                return true;

            if (genericDefinition.IsInterface && type.GetInterfaces().Any(i => IsGenericTypeOf(i, genericDefinition)))
                return true;

            if (type.BaseType != null && IsGenericTypeOf(type.BaseType, genericDefinition))
                return true;

            return false;
        }

        public static bool IsGenericTypeOf(this Type type, Type genericDefinition, out Type[] genericParameters)
        {
            genericParameters = EmptyTypes;

            var isMatch = type.IsGenericType &&
                          type.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();

            if (!isMatch && type.BaseType != null)
                isMatch = type.BaseType.IsGenericTypeOf(genericDefinition, out genericParameters);

            if (!isMatch && genericDefinition.IsInterface)
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Length != 0)
                {
                    foreach (var i in interfaces)
                    {
                        if (i.IsGenericTypeOf(genericDefinition, out genericParameters))
                        {
                            isMatch = true;
                            break;
                        }
                    }
                }
            }

            if (isMatch && !genericParameters.Any())
                genericParameters = type.GetGenericArguments();


            return isMatch;
        }
    }
}