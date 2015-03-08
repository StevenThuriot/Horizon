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
    static class TypeInfo<T>
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
                    var caller = MethodCaller.Create(methodInfo);
                    methods.Add(caller);
                }
            }
            
            Methods = methods.OrderBy(x => x is GenericMethodCaller)//this will make sure non-generic caller are prefered.
                             .ToLookup(x => x.Name, x => x);
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

        public static object Call(T instance, string methodName, IEnumerable<object> args)
        {
            var methods = Methods[methodName];
            return CallerSelector.Call(instance, args, methods);
        }

        public static object Call(string methodName, IEnumerable<object> args)
        {
            var methods = Methods[methodName];
            return CallerSelector.Call(args, methods);
        }

        public static bool TryCall(T instance, string methodName, IEnumerable<object> args, out object result)
        {
            var methods = Methods[methodName];
            return CallerSelector.TryCall(instance, args, methods, out result);
        }

        public static bool TryCall(string methodName, IEnumerable<object> args, out object result)
        {
            var methods = Methods[methodName];
            return CallerSelector.TryCall(args, methods, out result);
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
        
        public static IEnumerable<MethodCaller> GetMethod(string method)
        {
            return Methods[method];
        }








        public static bool CanImplicitConvert(T instance, Type type)
        {
            var method = Findop_Implicit(type);
            return method != null;
        }

        public static dynamic ImplicitConvert(T instance, Type type)
        {
            var method = Findop_Implicit(type);

            if (method == null) throw new ArgumentException("Invalid implicit conversion");

            var arguments = new dynamic[] { instance };
            return method.Call(arguments);
        }

        public static bool TryImplicitConvert(T instance, Type type, out dynamic result)
        {
            var methods = Methods["op_Implicit"];
            var method = methods.FirstOrDefault(n => n.ReturnType == type && n.ParameterTypes[0].ParameterType == Constants.Typed<T>.OwnerType);

            if (method == null)
            {
                result = null;
                return false;
            }

            var arguments = new dynamic[] { instance };

            result = method.Call(arguments);
            return true;
        }

        private static MethodCaller Findop_Implicit(Type type)
        {
            var methods = Methods["op_Implicit"];
            var owner = Constants.Typed<T>.OwnerType;
            var method = methods.FirstOrDefault(x => x.ReturnType == type && x.ParameterTypes[0].ParameterType == owner);

            return method;
        }
    }
}