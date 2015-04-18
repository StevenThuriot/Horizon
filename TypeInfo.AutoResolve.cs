using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Horizon
{
    static partial class TypeInfo
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

        public static void RaiseEvent<T>(this T instance, string @event, params dynamic[] arguments)
        {
            TypeInfo<T>.RaiseEvent(instance, @event, arguments);
        }

        public static void AddEventHandler<T>(this T instance, string @event, params Delegate[] delegates)
        {
            TypeInfo<T>.AddEventHandler(instance, @event, delegates);
        }

        public static void RemoveEventHandler<T>(this T instance, string @event, params Delegate[] delegates)
        {
            TypeInfo<T>.RemoveEventHandler(instance, @event, delegates);
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

        public static bool TryCall<T>(this T instance, InvokeMemberBinder binder, IEnumerable<object> args, out object result)
        {
            return TypeInfo<T>.TryCall(instance, binder, args, out result);
        }

        public static object Call<T>(this T instance, string methodName, IEnumerable<object> args)
        {
            return TypeInfo<T>.Call(instance, methodName, args);
        }

        public static bool TryCall<T>(this T instance, string methodName, IEnumerable<object> args, out object result)
        {
            return TypeInfo<T>.TryCall(instance, methodName, args, out result);
        }
        
        public static bool CanImplicitConvert<T>(this T instance, Type type)
        {
            return TypeInfo<T>.CanImplicitConvert(instance, type);
        }

        public static dynamic ImplicitConvert<T>(this T instance, Type type)
        {
            return TypeInfo<T>.ImplicitConvert(instance, type);
        }

        public static bool TryImplicitConvert<T>(this T instance, Type type, out dynamic result)
        {
            return TypeInfo<T>.TryImplicitConvert(instance, type, out result);
		}

        public static IEnumerable<ICaller> GetMethod<T>(this T instance, string method)
		{
			return TypeInfo<T>.GetMethod(method);
        }

        public static bool HasEvent<T>(this T instance, string @event)
        {
            return TypeInfo<T>.HasEvent(@event);
        }

        public static IEventCaller GetEvent<T>(this T instance, string @event)
        {
            return TypeInfo<T>.GetEvent(@event);
        }

        public static bool TryRaiseEvent<T>(this T instance, string @event, params dynamic[] arguments)
        {
            return TypeInfo<T>.TryRaiseEvent(instance, @event, arguments);
        }

        public static bool TryAddEventHandler<T>(this T instance, string @event, params Delegate[] delegates)
        {
            return TypeInfo<T>.TryAddEventHandler(instance, @event, delegates);
        }

        public static bool TryRemoveEventHandler<T>(this T instance, string @event, params Delegate[] delegates)
        {
            return TypeInfo<T>.TryRemoveEventHandler(instance, @event, delegates);
        }







        private static readonly Dictionary<Type, Func<object[], dynamic>> _creatorCache = new Dictionary<Type, Func<object[], dynamic>>();
		public static dynamic Create(Type type, params object[] args)
		{
		    Func<object[], dynamic> creator;
		    if (_creatorCache.TryGetValue(type, out creator))
		        return creator(args);

		    var info = typeof (TypeInfo<>).MakeGenericType(type);
		    var parameter = Expression.Parameter(Constants.ObjectArrayType, "arguments");
		    var call = Expression.Call(info, "Create", null, parameter);
		    var lambda = Expression.Lambda<Func<object[], dynamic>>(call, parameter);
            _creatorCache[type] = creator = lambda.Compile();

		    return creator(args);
		}

        public static T Create<T>(params object[] args)
        {
            return TypeInfo<T>.Create(args);
        }

        public static bool TryCreate<T>(out T instance, params object[] args)
        {
            return TypeInfo<T>.TryCreate(out instance, args);
        }
	}
}