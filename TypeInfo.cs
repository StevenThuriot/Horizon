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
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Invocation
{
    class TypeInfo<T>
    {
        public static readonly ILookup<string, MethodCaller> Methods;
        private static readonly Dictionary<string, Lazy<Func<T, object>>> Fields = new Dictionary<string, Lazy<Func<T, object>>>();
        private static readonly Dictionary<string, Lazy<Func<T, object>>> Properties = new Dictionary<string, Lazy<Func<T, object>>>();

        static TypeInfo()
        {
            var methods = new List<MethodCaller>();

            foreach (var member in Constants.Typed<T>.OwnerType.GetMembers())
            {
                var key = member.Name;

                if ((MemberTypes.Property & member.MemberType) == MemberTypes.Property)
                {
                    Properties[key] = InvokeHelper<T>.CreateLazy((PropertyInfo)member);
                }
                else if ((MemberTypes.Field & member.MemberType) == MemberTypes.Field)
                {
                    Fields[key] = InvokeHelper<T>.CreateLazy((FieldInfo)member);
                }
                else if ((MemberTypes.Method & member.MemberType) == MemberTypes.Method)
                {
                    var methodInfo = (MethodInfo) member;
                    var caller = new MethodCaller(methodInfo);
                    methods.Add(caller);
                }
            }

            Methods = methods.ToLookup(x => x.Name, x => x);
        }

        public static dynamic GetProperty(T instance, string property)
        {
            return Properties[property].Value(instance);
        }

        public static dynamic GetField(T instance, string field)
        {
            return Fields[field].Value(instance);
        }
        
        public static dynamic Call(T instance, InvokeMemberBinder binder, IList<object> args)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.Call(instance, binder, args, methods);
        }
        
        public static dynamic Call(InvokeMemberBinder binder, IList<object> args)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.Call(binder, args, methods);
        }
    }
}