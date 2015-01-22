﻿#region License
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
        private static readonly Dictionary<string, Lazy<Func<T, object>>> GetFields = new Dictionary<string, Lazy<Func<T, object>>>();
        private static readonly Dictionary<string, Lazy<Func<T, object>>> GetProperties = new Dictionary<string, Lazy<Func<T, object>>>();
        private static readonly Dictionary<string, Lazy<Action<T, object>>> SetFields = new Dictionary<string, Lazy<Action<T, object>>>();
        private static readonly Dictionary<string, Lazy<Action<T, object>>> SetProperties = new Dictionary<string, Lazy<Action<T, object>>>();

        static TypeInfo()
        {
            var methods = new List<MethodCaller>();

            foreach (var member in Constants.Typed<T>.OwnerType.GetMembers())
            {
                var key = member.Name;

                if ((MemberTypes.Property & member.MemberType) == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)member;
                    if (propertyInfo.CanWrite)
                        SetProperties[key] = InvokeHelper<T>.CreateSetterLazy(propertyInfo);

                    if (propertyInfo.CanRead)
                        GetProperties[key] = InvokeHelper<T>.CreateGetterLazy(propertyInfo);
                }
                else if ((MemberTypes.Field & member.MemberType) == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo)member;

                    if (!fieldInfo.IsInitOnly)
                        SetFields[key] = InvokeHelper<T>.CreateSetterLazy(fieldInfo);

                    GetFields[key] = InvokeHelper<T>.CreateGetterLazy(fieldInfo);
                }
                else if ((MemberTypes.Method & member.MemberType) == MemberTypes.Method)
                {
                    var methodInfo = (MethodInfo)member;
                    var caller = new MethodCaller(methodInfo);
                    methods.Add(caller);
                }
            }

            Methods = methods.ToLookup(x => x.Name, x => x);
        }

        public static object GetProperty(T instance, string property)
        {
            return GetProperties[property].Value(instance);
        }

        public static object GetField(T instance, string field)
        {
            return GetFields[field].Value(instance);
        }

        public static void SetProperty(T instance, string property, object value)
        {
            SetProperties[property].Value(instance, value);
        }

        public static void SetField(T instance, string field, object value)
        {
            SetFields[field].Value(instance, value);
        }

        public static object Call(T instance, InvokeMemberBinder binder, IList<object> args)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.Call(instance, binder, args, methods);
        }

        public static object Call(InvokeMemberBinder binder, IList<object> args)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.Call(binder, args, methods);
        }





        public static bool HasField(string field)
        {
            return HasGetterField(field); //No need to check setter
        }

        public static bool HasProperty(string property)
        {
            return HasGetterProperty(property) || HasSetterProperty(property);
        }

        public static bool HasGetterProperty(string property)
        {
            return GetProperties.ContainsKey(property);
        }
        public static bool HasSetterProperty(string property)
        {
            return SetProperties.ContainsKey(property);
        }

        public static bool HasGetterField(string field)
        {
            return GetFields.ContainsKey(field);
        }
        public static bool HasSetterField(string field)
        {
            return SetFields.ContainsKey(field);
        }

        public static bool HasMethod(string method)
        {
            return Methods.Contains(method);
        }
    }
}