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
    static class TypeInfo
    {
        public static object GetProperty<T>(this T instance, string property)
        {
            return TypeInfo<T>.GetProperty(instance, property);
        }

        public static object GetField<T>(this T instance, string field)
        {
            return TypeInfo<T>.GetField(instance, field);
        }

        public static void SetProperty<T>(this T instance, string property, object value)
        {
            TypeInfo<T>.SetProperty(instance, property, value);
        }

        public static void SetField<T>(this T instance, string field, object value)
        {
            TypeInfo<T>.SetField(instance, field, value);
        }

        public static bool TryGetProperty<T>(this T instance, string property, out object result)
        {
            return TypeInfo<T>.TryGetProperty(instance, property, out result);
        }

        public static bool TryGetField<T>(this T instance, string field, out object result)
        {
            return TypeInfo<T>.TryGetProperty(instance, field, out result);
        }

        public static object GetIndexer<T>(this T instance, object[] indexes)
        {
            return TypeInfo<T>.GetIndexer(instance, indexes);
        }

        public static bool TryGetIndexer<T>(this T instance, object[] indexes, out object result)
        {
            return TypeInfo<T>.TryGetIndexer(instance, indexes, out result);
        }

        public static bool TrySetProperty<T>(this T instance, string property, object value)
        {
            return TypeInfo<T>.TrySetProperty(instance, property, value);
        }

        public static bool TrySetField<T>(this T instance, string field, object value)
        {
            return TypeInfo<T>.TrySetField(instance, field, value);
        }

        public static void SetIndexer<T>(this T instance, object[] indexes, object value)
        {
            TypeInfo<T>.SetIndexer(instance, indexes, value);
        }

        public static bool TrySetIndexer<T>(this T instance, object[] indexes, object value)
        {
            return TypeInfo<T>.TrySetIndexer(instance, indexes, value);
        }

        public static object Call<T>(this T instance, InvokeMemberBinder binder, IEnumerable<object> args)
        {
            return TypeInfo<T>.Call(instance, binder, args);
        }

        public static bool TryCall<T>(this T instance, InvokeMemberBinder binder, IEnumerable<object> args,
                                      out object result)
        {
            return TypeInfo<T>.TryCall(instance, binder, args, out result);
        }
    }

    class TypeInfo<T>
    {
        private static readonly ILookup<string, MethodCaller> Methods;

        private static readonly Dictionary<string, Lazy<Func<T, object>>> GetFields =
            new Dictionary<string, Lazy<Func<T, object>>>();

        private static readonly Dictionary<string, Lazy<Func<T, object>>> GetProperties =
            new Dictionary<string, Lazy<Func<T, object>>>();

        private static readonly Dictionary<string, Lazy<Action<T, object>>> SetFields =
            new Dictionary<string, Lazy<Action<T, object>>>();

        private static readonly Dictionary<string, Lazy<Action<T, object>>> SetProperties =
            new Dictionary<string, Lazy<Action<T, object>>>();

        static TypeInfo()
        {
            var methods = new List<MethodCaller>();

            foreach (var member in Constants.Typed<T>.OwnerType.GetMembers())
            {
                var key = member.Name;

                if ((MemberTypes.Property & member.MemberType) == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo) member;
                    if (propertyInfo.CanWrite)
                        SetProperties[key] = InvokeHelper<T>.CreateSetterLazy(propertyInfo);

                    if (propertyInfo.CanRead)
                        GetProperties[key] = InvokeHelper<T>.CreateGetterLazy(propertyInfo);
                }
                else if ((MemberTypes.Field & member.MemberType) == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo) member;

                    if (!fieldInfo.IsInitOnly)
                        SetFields[key] = InvokeHelper<T>.CreateSetterLazy(fieldInfo);

                    GetFields[key] = InvokeHelper<T>.CreateGetterLazy(fieldInfo);
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

        public static bool TryGetProperty(T instance, string property, out object result)
        {
            Lazy<Func<T, object>> getter;
            if (GetProperties.TryGetValue(property, out getter))
            {
                result = getter.Value(instance);
                return true;
            }

            result = null;
            return false;
        }

        public static bool TryGetField(T instance, string field, out object result)
        {
            Lazy<Func<T, object>> getter;
            if (GetFields.TryGetValue(field, out getter))
            {
                result = getter.Value(instance);
                return true;
            }

            result = null;
            return false;
        }

        public static object GetIndexer(T instance, object[] indexes)
        {
            var methods = Methods["get_Item"];
            return CallerSelector.GetIndexer(instance, methods, indexes);
        }

        public static bool TryGetIndexer(T instance, object[] indexes, out object result)
        {
            var methods = Methods["get_Item"];
            return CallerSelector.TryGetIndexer(instance, methods, indexes, out result);
        }

        public static bool TrySetProperty(T instance, string property, object value)
        {
            Lazy<Action<T, object>> setter;
            if (SetProperties.TryGetValue(property, out setter))
            {
                setter.Value(instance, value);
                return true;
            }

            return false;
        }

        public static bool TrySetField(T instance, string field, object value)
        {
            Lazy<Action<T, object>> setter;
            if (SetFields.TryGetValue(field, out setter))
            {
                setter.Value(instance, value);
                return true;
            }

            return false;
        }

        public static void SetIndexer(T instance, object[] indexes, object value)
        {
            var methods = Methods["set_Item"];
            CallerSelector.SetIndexer(instance, methods, indexes, value);
        }

        public static bool TrySetIndexer(T instance, object[] indexes, object value)
        {
            var methods = Methods["set_Item"];
            return CallerSelector.TrySetIndexer(instance, methods, indexes, value);
        }

        public static object Call(T instance, InvokeMemberBinder binder, IEnumerable<object> args)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.Call(instance, binder, args, methods);
        }

        public static object Call(InvokeMemberBinder binder, IEnumerable<object> args)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.Call(binder, args, methods);
        }

        public static bool TryCall(T instance, InvokeMemberBinder binder, IEnumerable<object> args, out object result)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.TryCall(instance, binder, args, methods, out result);
        }

        public static bool TryCall(InvokeMemberBinder binder, IEnumerable<object> args, out object result)
        {
            var methods = Methods[binder.Name];
            return CallerSelector.TryCall(binder, args, methods, out result);
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