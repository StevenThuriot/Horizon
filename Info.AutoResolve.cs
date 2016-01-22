using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Horizon
{
    static partial class Info
    {
        public static object GetProperty<T>(this T instance, string property) => Info<T>.GetProperty(instance, property);

        public static object GetField<T>(this T instance, string field) => Info<T>.GetField(instance, field);

        public static void SetProperty<T>(this T instance, string property, object value) => Info<T>.SetProperty(instance, property, value);

        public static void SetField<T>(this T instance, string field, object value) => Info<T>.SetField(instance, field, value);

        public static void RaiseEvent<T>(this T instance, string @event, params dynamic[] arguments) => Info<T>.RaiseEvent(instance, @event, arguments);

        public static void AddEventHandler<T>(this T instance, string @event, params Delegate[] delegates) => Info<T>.AddEventHandler(instance, @event, delegates);

        public static void RemoveEventHandler<T>(this T instance, string @event, params Delegate[] delegates) => Info<T>.RemoveEventHandler(instance, @event, delegates);

        public static bool TryGetProperty<T>(this T instance, string property, out object result) => Info<T>.TryGetProperty(instance, property, out result);

        public static bool TryGetField<T>(this T instance, string field, out object result) => Info<T>.TryGetProperty(instance, field, out result);

        public static object GetIndexer<T>(this T instance, object[] indexes) => Info<T>.GetIndexer(instance, indexes);

        public static bool TryGetIndexer<T>(this T instance, object[] indexes, out object result) => Info<T>.TryGetIndexer(instance, indexes, out result);

        public static bool TrySetProperty<T>(this T instance, string property, object value) => Info<T>.TrySetProperty(instance, property, value);

        public static bool TrySetField<T>(this T instance, string field, object value) => Info<T>.TrySetField(instance, field, value);

        public static void SetIndexer<T>(this T instance, object[] indexes, object value) => Info<T>.SetIndexer(instance, indexes, value);
        
        public static bool TrySetIndexer<T>(this T instance, object[] indexes, object value) => Info<T>.TrySetIndexer(instance, indexes, value);

        public static object Call<T>(this T instance, InvokeMemberBinder binder, IEnumerable<object> args) => Info<T>.Call(instance, binder, args);

        public static bool TryCall<T>(this T instance, InvokeMemberBinder binder, IEnumerable<object> args, out object result) => Info<T>.TryCall(instance, binder, args, out result);

        public static object Call<T>(this T instance, string methodName, IEnumerable<object> args) => Info<T>.Call(instance, methodName, args);

        public static bool TryCall<T>(this T instance, string methodName, IEnumerable<object> args, out object result) => Info<T>.TryCall(instance, methodName, args, out result);

        public static bool CanImplicitConvert<T>(this T instance, Type type) => Info<T>.CanImplicitConvert(instance, type);

        public static dynamic ImplicitConvert<T>(this T instance, Type type) => Info<T>.ImplicitConvert(instance, type);

        public static bool TryImplicitConvert<T>(this T instance, Type type, out dynamic result) => Info<T>.TryImplicitConvert(instance, type, out result);

        public static bool HasField<T>(this T instance, string field) => Info<T>.HasField(field);

        public static bool HasProperty<T>(this T instance, string property) => Info<T>.HasProperty(property);

        public static bool HasGetterProperty<T>(this T instance, string property) => Info<T>.HasGetterProperty(property);

        public static bool HasSetterProperty<T>(this T instance, string property) => Info<T>.HasSetterProperty(property);

        public static bool HasGetterField<T>(this T instance, string field) => Info<T>.HasGetterField(field);

        public static bool HasSetterField<T>(this T instance, string field) => Info<T>.HasSetterField(field);

        public static bool HasMethod<T>(this T instance, string method) => Info<T>.HasMethod(method);

        public static IEnumerable<IMethodCaller> GetMethod<T>(this T instance, string method) => Info<T>.GetMethod(method);

        public static IMethodCaller GetSpecificMethod<T>(this T instance, string method, params Type[] arguments) => Info<T>.GetSpecificMethod(method, arguments);

        public static IConstructorCaller GetConstructor<T>(this T instance, params Type[] arguments) => Info<T>.GetConstructor(arguments);

        public static bool HasEvent<T>(this T instance, string @event) => Info<T>.HasEvent(@event);

        public static IEventCaller GetEvent<T>(this T instance, string @event) => Info<T>.GetEvent(@event);

        public static bool TryRaiseEvent<T>(this T instance, string @event, params dynamic[] arguments) => Info<T>.TryRaiseEvent(instance, @event, arguments);

        public static bool TryAddEventHandler<T>(this T instance, string @event, params Delegate[] delegates) => Info<T>.TryAddEventHandler(instance, @event, delegates);

        public static bool TryRemoveEventHandler<T>(this T instance, string @event, params Delegate[] delegates) => Info<T>.TryRemoveEventHandler(instance, @event, delegates);

        public static object GetValue<T>(this T instance, string propertyOrField) => Info<T>.GetValue(instance, propertyOrField);

        public static bool TryGetValue<T>(this T instance, string propertyOrField, out object result) => Info<T>.TryGetValue(instance, propertyOrField, out result);

        public static void SetValue<T>(this T instance, string propertyOrField, object value) => Info<T>.SetValue(instance, propertyOrField, value);

        public static bool TrySetValue<T>(this T instance, string propertyOrField, object value) => Info<T>.TrySetValue(instance, propertyOrField, value);

        public static object GetValue<T>(this T instance, GetMemberBinder binder) => GetValue(instance, binder.Name);

        public static bool TryGetValue<T>(this T instance, GetMemberBinder binder, out object result) => TryGetValue(instance, binder.Name, out result);

        public static void SetValue<T>(this T instance, SetMemberBinder binder, object value) => SetValue(instance, binder.Name, value);

        public static bool TrySetValue<T>(this T instance, SetMemberBinder binder, object value) => TrySetValue(instance, binder.Name, value);

        public static object GetProperty<T>(this T instance, GetMemberBinder binder) => GetProperty(instance, binder.Name);

        public static bool TryGetProperty<T>(this T instance, GetMemberBinder binder, out object result) => TryGetProperty(instance, binder.Name, out result);

        public static void SetProperty<T>(this T instance, SetMemberBinder binder, object value) => SetProperty(instance, binder.Name, value);

        public static bool TrySetProperty<T>(this T instance, SetMemberBinder binder, object value) => TrySetProperty(instance, binder.Name, value);

        public static void SetField<T>(this T instance, SetMemberBinder binder, object value) => SetField(instance, binder.Name, value);

        public static bool TrySetField<T>(this T instance, SetMemberBinder binder, object value) => TrySetField(instance, binder.Name, value);

        public static T Create<T>(params object[] args) => Info<T>.Create(args);

        public static bool TryCreate<T>(out T instance, params object[] args) => Info<T>.TryCreate(out instance, args);


        static readonly Dictionary<Type, Func<object[], dynamic>> _creatorCache = new Dictionary<Type, Func<object[], dynamic>>();
        public static dynamic Create(Type type, params object[] args)
        {
            Func<object[], dynamic> creator;
            if (_creatorCache.TryGetValue(type, out creator))
                return creator(args);

            var info = typeof(Info<>).MakeGenericType(type);
            var parameter = Expression.Parameter(typeof(object[]), "arguments");
            var call = Expression.Call(info, "Create", null, parameter);
            var lambda = Expression.Lambda<Func<object[], dynamic>>(call, parameter);
            _creatorCache[type] = creator = lambda.Compile();

            return creator(args);
        }
    }
}