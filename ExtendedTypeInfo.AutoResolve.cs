﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Horizon
{
    static partial class TypeInfo
    {
        public static class Extended
        {
            public static IEnumerable<ICaller> Methods<T>(T instance)
            {
                return TypeInfo<T>.Extended.Methods;
            }

            public static IEnumerable<ICaller> Constructors<T>(T instance)
            {
                return TypeInfo<T>.Extended.Constructors;
            }

            public static IEnumerable<IEventCaller> Events<T>(T instance)
            {
                return TypeInfo<T>.Extended.Events;
            }


            public static IEnumerable<KeyValuePair<string, Lazy<Func<T, object>>>> Getters<T>(T instance)
            {
                return TypeInfo<T>.Extended.Getters;
            }

            public static IEnumerable<KeyValuePair<string, Lazy<Action<T, object>>>> Setters<T>(T instance)
            {
                return TypeInfo<T>.Extended.Setters;
            }







            public static IEnumerable<ICaller> Methods(Type type)
            {
                return ResolveCallForType(type, _methodCache);
            }

            public static IEnumerable<ICaller> Constructors(Type type)
            {
                return ResolveCallForType(type, _ctorCache);
            }

            public static IEnumerable<IEventCaller> Events(Type type)
            {
                return ResolveCallForType(type, _eventCache);
            }
            
            public static dynamic Getters(Type type)
            {
                return DynamicResolveCallForType(type, _getterCache);
            }

            public static dynamic Setters(Type type)
            {
                return DynamicResolveCallForType(type, _setterCache);
            }








            private static readonly Dictionary<Type, Func<IEnumerable<MethodCaller>>> _methodCache = new Dictionary<Type, Func<IEnumerable<MethodCaller>>>();
            private static readonly Dictionary<Type, Func<IEnumerable<ConstructorCaller>>> _ctorCache = new Dictionary<Type, Func<IEnumerable<ConstructorCaller>>>();
            private static readonly Dictionary<Type, Func<IEnumerable<EventCaller>>> _eventCache = new Dictionary<Type, Func<IEnumerable<EventCaller>>>();
            private static readonly Dictionary<Type, dynamic> _getterCache = new Dictionary<Type, dynamic>();
            private static readonly Dictionary<Type, dynamic> _setterCache = new Dictionary<Type, dynamic>();

            private static TResult ResolveCallForType<TResult>(Type type, Dictionary<Type, Func<TResult>> cache, [CallerMemberName]string propertyName = null)
            {
                Func<TResult> enumerator;
                if (cache.TryGetValue(type, out enumerator))
                    return enumerator();

                var info = typeof (TypeInfo<>.Extended).MakeGenericType(type);
                var property = Expression.Property(null, info, propertyName);
                var lambda = Expression.Lambda<Func<TResult>>(property);

                cache[type] = enumerator = lambda.Compile();

                return enumerator();
            }

            private static dynamic DynamicResolveCallForType(Type type, Dictionary<Type, dynamic> cache, [CallerMemberName]string propertyName = null)
            {
                dynamic enumerator;
                if (cache.TryGetValue(type, out enumerator))
                    return enumerator();

                var info = typeof (TypeInfo<>.Extended).MakeGenericType(type);
                var property = Expression.Property(null, info, propertyName);
                var lambda = Expression.Lambda(property);

                cache[type] = enumerator = lambda.Compile();

                return enumerator();
            }
        }
    }
}
