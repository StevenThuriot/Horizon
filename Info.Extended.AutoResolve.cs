﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Horizon
{
    static partial class Info
    {
        public static class Extended
        {
            public static IEnumerable<IMethodCaller> Methods<T>(T instance) => Info<T>.Extended.Methods;

            public static IEnumerable<IConstructorCaller> Constructors<T>(T instance) => Info<T>.Extended.Constructors;

            public static IConstructorCaller DefaultConstructor<T>(T instance) => Info<T>.Extended.DefaultConstructor;

            public static IEnumerable<IEventCaller> Events<T>(T instance) => Info<T>.Extended.Events;


            public static IEnumerable<IPropertyCaller<T>> Properties<T>(T instance) => Info<T>.Extended.Properties;

            public static IEnumerable<IMemberCaller<T>> Fields<T>(T instance) => Info<T>.Extended.Fields;

            public static IEnumerable<IMemberCaller<T>> Members<T>(T instance)
            {
                foreach (var caller in Info<T>.Extended.Properties)
                    yield return caller;

                foreach (var caller in Info<T>.Extended.Fields)
                    yield return caller;
            }







            public static IEnumerable<IMethodCaller> Methods(Type type) => ResolveCallForType(type, _methodCache);

            public static IEnumerable<IConstructorCaller> Constructors(Type type) => ResolveCallForType(type, _ctorCache);

            public static IConstructorCaller DefaultConstructor(Type type) => ResolveSpecificCaller(Constructors(type), Type.EmptyTypes);

            public static IEnumerable<IEventCaller> Events(Type type) => ResolveCallForType(type, _eventCache);

            public static IEnumerable<IPropertyCaller> Properties(Type type) => ResolveCallForType(type, _propertyCache);

            public static IEnumerable<IMemberCaller> Fields(Type type) => ResolveCallForType(type, _fieldCache);

            public static IEnumerable<IMemberCaller> Members(Type type)
            {
                foreach (var property in Properties(type))
                    yield return property;

                foreach (var property in Fields(type))
                    yield return property;
            }







            static readonly Dictionary<Type, Func<IEnumerable<IMethodCaller>>> _methodCache = new Dictionary<Type, Func<IEnumerable<IMethodCaller>>>();
            static readonly Dictionary<Type, Func<IEnumerable<IConstructorCaller>>> _ctorCache = new Dictionary<Type, Func<IEnumerable<IConstructorCaller>>>();
            static readonly Dictionary<Type, Func<IEnumerable<IEventCaller>>> _eventCache = new Dictionary<Type, Func<IEnumerable<IEventCaller>>>();
            static readonly Dictionary<Type, Func<IEnumerable<IPropertyCaller>>> _propertyCache = new Dictionary<Type, Func<IEnumerable<IPropertyCaller>>>();
            static readonly Dictionary<Type, Func<IEnumerable<IMemberCaller>>> _fieldCache = new Dictionary<Type, Func<IEnumerable<IMemberCaller>>>();

            static TResult ResolveCallForType<TResult>(Type type, Dictionary<Type, Func<TResult>> cache, [CallerMemberName]string propertyName = null)
            {
                Func<TResult> enumerator;
                if (cache.TryGetValue(type, out enumerator))
                    return enumerator();

                var info = typeof(Info<>.Extended).MakeGenericType(type);
                var property = Expression.Property(null, info, propertyName);
                var lambda = Expression.Lambda<Func<TResult>>(property);

                cache[type] = enumerator = lambda.Compile();

                return enumerator();
            }
            
            //Simple specific version of the caller selector which enforces types and argument count.
            internal static TCaller ResolveSpecificCaller<TCaller>(IEnumerable<TCaller> callers, IReadOnlyList<Type> arguments, bool throwWhenNotfound = true)
                where TCaller : ICaller
            {
                foreach (var caller in callers.Where(x => x.ParameterTypes.Count == arguments.Count))
                {
                    var parameterTypes = caller.ParameterTypes;

                    var match = true;
                    for (var i = 0; i < arguments.Count; i++)
                    {
                        var argument = arguments[i];
                        var simpleParameterInfo = parameterTypes[i];
                        if (simpleParameterInfo.ParameterType == argument)
                            continue;

                        match = false;
                        break;
                    }

                    if (match)
                        return caller;
                }

                if (throwWhenNotfound)
                    throw new ArgumentException("No matching caller found.");

                return default(TCaller);
            }
        }
    }
}