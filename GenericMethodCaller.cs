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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Invocation
{
    class GenericMethodCaller : MethodCaller
    {
        private readonly Dictionary<TypeHash, Delegate> _cache = new Dictionary<TypeHash, Delegate>();

        internal GenericMethodCaller(MethodInfo info)
            : base(info, GetParameters(info))
        {
        }

        private static IEnumerable<SimpleParameterInfo> GetParameters(MethodInfo info)
        {
            foreach (var parameterInfo in info.GetParameters())
            {
                var name = parameterInfo.Name;
                var hasDefaultValue = parameterInfo.HasDefaultValue;
                var defaultValue = hasDefaultValue ? parameterInfo.DefaultValue : null;

                var type = parameterInfo.ParameterType;

                var genericTypeDefinition = type;

                if (!type.IsGenericParameter)
                {
                    if (type.ContainsGenericParameters)
                    {
                        Debugger.Break();
                        genericTypeDefinition = type.GetGenericTypeDefinition();
                        
                    }
                }

                yield return new SimpleParameterInfo(name, defaultValue, genericTypeDefinition, type, hasDefaultValue); ;
            }
        }

        public override object Call(IEnumerable<dynamic> values)
        {
            var genericArguments = Info.GetGenericArguments();
            
            var types = genericArguments.Select(x => FindTypeFor(x, values))
                                        .ToArray();

            var hash = new TypeHash(types);

            Delegate @delegate;
            if (!_cache.TryGetValue(hash, out @delegate))
            {
                var caller = Info.MakeGenericMethod(types);
                @delegate = caller.Build();
                _cache[hash] = @delegate;
            }
            
            var arguments = values.ToArray();
            return @delegate.FastInvoke(arguments);
        }

        private Type FindTypeFor(Type type, IEnumerable<object> values)
        {
            for (int i = 0; i < ParameterTypes.Count; i++)
            {
                var spi = ParameterTypes[i];
                var originalType = spi.OriginalParameterType;

                if (originalType == spi.ParameterType) continue; //Not it!

                var value = values.ElementAt(i);
                var valueType = value == null ? typeof(object) : value.GetType();

                Type genericType;
                if (CheckGeneric(type, originalType, valueType, out genericType))
                {
                    return genericType;
                }
            }
            
            Debugger.Break();
            throw new NotSupportedException();
        }

        private static bool CheckGeneric(Type type, Type originalType, Type valueType, out Type genericType)
        {
            if (type == originalType)
            {
                genericType = valueType;
                return true;
            }

            var parameters = originalType.GetGenericArguments();
            var vparameters = valueType.GetGenericArguments();

            if (parameters.Length == 0 || vparameters.Length != parameters.Length)
            {
                genericType = null;
                return false;
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var vparam = vparameters[i];

                Type sub;
                if (CheckGeneric(type, parameter, vparam, out sub))
                {
                    genericType = sub;
                    return true;
                }
            }

            genericType = null;
            return false;
        }
    }
}