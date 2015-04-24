using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Horizon
{
    static partial class TypeInfo
    {
        public static class Extended
        {
            public static IEnumerable<IMethodCaller> Methods<T>(T instance)
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


            public static IEnumerable<IPropertyCaller<T>> Properties<T>(T instance)
            {
                return TypeInfo<T>.Extended.Properties;
            }

            public static IEnumerable<IMemberCaller<T>> Fields<T>(T instance)
            {
                return TypeInfo<T>.Extended.Fields;
            }

            public static IEnumerable<IMemberCaller<T>> Members<T>(T instance)
            {
                foreach (var caller in TypeInfo<T>.Extended.Properties)
                    yield return caller;

                foreach (var caller in TypeInfo<T>.Extended.Fields)
                    yield return caller;
            }







            public static IEnumerable<IMethodCaller> Methods(Type type)
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

            public static IEnumerable<IPropertyCaller> Properties(Type type)
            {
                return ResolveCallForType(type, _propertyCache);
            }

            public static IEnumerable<IMemberCaller> Fields(Type type)
            {
                return ResolveCallForType(type, _fieldCache);
            }

            public static IEnumerable<IMemberCaller> Members(Type type)
            {
                foreach (var property in Properties(type))
                    yield return property;

                foreach (var property in Fields(type))
                    yield return property;
            }







            private static readonly Dictionary<Type, Func<IEnumerable<IMethodCaller>>> _methodCache = new Dictionary<Type, Func<IEnumerable<IMethodCaller>>>();
            private static readonly Dictionary<Type, Func<IEnumerable<ICaller>>> _ctorCache = new Dictionary<Type, Func<IEnumerable<ICaller>>>();
            private static readonly Dictionary<Type, Func<IEnumerable<IEventCaller>>> _eventCache = new Dictionary<Type, Func<IEnumerable<IEventCaller>>>();
            private static readonly Dictionary<Type, Func<IEnumerable<IPropertyCaller>>> _propertyCache = new Dictionary<Type, Func<IEnumerable<IPropertyCaller>>>();
            private static readonly Dictionary<Type, Func<IEnumerable<IMemberCaller>>> _fieldCache = new Dictionary<Type, Func<IEnumerable<IMemberCaller>>>();

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
        }
    }
}
