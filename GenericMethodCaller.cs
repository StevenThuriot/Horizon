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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Invocation
{
    class GenericMethodCaller : MethodCaller
    {
        private readonly Dictionary<TypeHash, Delegate> _cache = new Dictionary<TypeHash, Delegate>();

        internal GenericMethodCaller(MethodInfo info)
            : base(info.GetGenericMethodDefinition(), GetParameters(info))
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
            var arguments = values.ToArray();

            var types = arguments.Cast<object>()
                                 .Select(x => ReferenceEquals(null, x) ? typeof (object) : x.GetType());

            var hash = new TypeHash(types);

            Delegate @delegate;
            if (!_cache.TryGetValue(hash, out @delegate))
            {
                @delegate = Info.BuildCallSite(arguments);
                _cache[hash] = @delegate;
            }

            return @delegate.FastInvoke(arguments);
        }
    }
}