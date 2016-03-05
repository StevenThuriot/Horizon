using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Horizon
{
    partial class Info
    {
        public static class Static
        {
            static IDictionary<Type, IInfoContainer> _cache = new Dictionary<Type, IInfoContainer>();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
            static IInfoContainer ResolveInfoContainer(Type type)
            {
                IInfoContainer container;

                if (!_cache.TryGetValue(type, out container))
                {
                    var containerType = typeof(InfoContainer<>).MakeGenericType(type);
                    _cache[type] = container = Info.Create(containerType);
                }

                return container;
            }

            public static IEnumerable<IMethodCaller> Methods(Type type) => ResolveInfoContainer(type).Methods.SelectMany(x => x);

            public static IEnumerable<IEventCaller> Events(Type type) => ResolveInfoContainer(type).Events.Values;

            public static IEnumerable<IPropertyCaller> Properties(Type type) => ResolveInfoContainer(type).Properties.Values;

            public static IEnumerable<IMemberCaller> Fields(Type type) => ResolveInfoContainer(type).Fields.Values;

            public static IEnumerable<IMemberCaller> Members(Type type)
            {
                foreach (var property in Properties(type))
                    yield return property;

                foreach (var property in Fields(type))
                    yield return property;
            }








#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members
            public static object GetProperty(Type type, string property) => ResolveInfoContainer(type).Properties[property].Get(null);

            public static object GetField(Type type, string field) => ResolveInfoContainer(type).Fields[field].Get(null);

            public static void SetProperty(Type type, string property, object value) => ResolveInfoContainer(type).Properties[property].Set(null, value);

            public static void SetField(Type type, string field, object value) => ResolveInfoContainer(type).Fields[field].Set(null, value);

            public static void RaiseEvent(Type type, string @event, params dynamic[] arguments) => ResolveInfoContainer(type).Events[@event].Raise((object) null, arguments);

            public static void AddEventHandler(Type type, string @event, params Delegate[] delegates) => ResolveInfoContainer(type).Events[@event].Add((object) null, delegates);

            public static void RemoveEventHandler(Type type, string @event, params Delegate[] delegates) => ResolveInfoContainer(type).Events[@event].Remove((object) null, delegates);

            public static object GetValue(Type type, string propertyOrField)
            {
                IPropertyCaller getter;
                if (ResolveInfoContainer(type).Properties.TryGetValue(propertyOrField, out getter))
                    return getter.Get(null);

                return ResolveInfoContainer(type).Fields[propertyOrField].Get(null);
            }


            public static bool TryGetValue(Type type, string propertyOrField, out object result)
            {
                IPropertyCaller prop;
                if (ResolveInfoContainer(type).Properties.TryGetValue(propertyOrField, out prop))
                    return prop.TryGet(null, out result);

                IMemberCaller field;
                if (ResolveInfoContainer(type).Fields.TryGetValue(propertyOrField, out field))
                    return field.TryGet(null, out result);

                result = null;
                return false;
            }


            public static void SetValue(Type type, string propertyOrField, object value)
            {
                IPropertyCaller prop;
                if (ResolveInfoContainer(type).Properties.TryGetValue(propertyOrField, out prop))
                {
                    prop.Set(null, value);
                }
                else
                {
                    ResolveInfoContainer(type).Fields[propertyOrField].Set(null, value);
                }
            }


            public static bool TrySetValue(Type type, string propertyOrField, object value)
            {
                IPropertyCaller prop;
                if (ResolveInfoContainer(type).Properties.TryGetValue(propertyOrField, out prop))
                    return prop.TrySet(null, value);

                IMemberCaller field;
                if (ResolveInfoContainer(type).Fields.TryGetValue(propertyOrField, out field))
                    return field.TrySet(null, value);

                return false;
            }


            public static object GetValue(Type type, GetMemberBinder binder) => GetValue(type, binder.Name);
            public static bool TryGetValue(Type type, GetMemberBinder binder, out object result) => TryGetValue(type, binder.Name, out result);

            public static void SetValue(Type type, SetMemberBinder binder, object value) => SetValue(type, binder.Name, value);
            public static bool TrySetValue(Type type, SetMemberBinder binder, object value) => TrySetValue(type, binder.Name, value);


            public static bool TryGetProperty(Type type, string property, out object result)
            {
                IPropertyCaller getter;
                if (ResolveInfoContainer(type).Properties.TryGetValue(property, out getter))
                    return getter.TryGet(null, out result);

                result = null;
                return false;
            }

            public static bool TryGetField(Type type, string field, out object result)
            {
                IMemberCaller getter;
                if (ResolveInfoContainer(type).Fields.TryGetValue(field, out getter))
                    return getter.TryGet(null, out result);

                result = null;
                return false;
            }

            public static object GetProperty(Type type, GetMemberBinder binder) => GetProperty(type, binder.Name);
            public static bool TryGetProperty(Type type, GetMemberBinder binder, out object result) => TryGetProperty(type, binder.Name, out result);

            public static object GetIndexer(Type type, object[] indexes)
            {
                var methods = ResolveInfoContainer(type).Indexers.Where(x => x.CanRead)
                                       .Select(x => x.GetGetCaller())
                                       .ToArray();

                return CallerSelector.GetIndexer((object) null, methods, indexes);
            }

            public static bool TryGetIndexer(Type type, object[] indexes, out object result)
            {
                var methods = ResolveInfoContainer(type).Indexers.Where(x => x.CanRead)
                                       .Select(x => x.GetGetCaller())
                                       .Where(x => x != null)
                                       .ToArray();

                return CallerSelector.TryGetIndexer((object) null, methods, indexes, out result);
            }

            public static bool TrySetProperty(Type type, string property, object value)
            {
                IPropertyCaller setter;
                return ResolveInfoContainer(type).Properties.TryGetValue(property, out setter) && setter.TrySet(null, value);
            }

            public static void SetProperty(Type type, SetMemberBinder binder, object value) => SetProperty(type, binder.Name, value);
            public static bool TrySetProperty(Type type, SetMemberBinder binder, object value) => TrySetProperty(type, binder.Name, value);

            public static bool TrySetField(Type type, string field, object value)
            {
                IMemberCaller setter;
                return ResolveInfoContainer(type).Fields.TryGetValue(field, out setter) && setter.TrySet(null, value);
            }

            public static void SetField(Type type, SetMemberBinder binder, object value) => SetField(type, binder.Name, value);
            public static bool TrySetField(Type type, SetMemberBinder binder, object value) => TrySetField(type, binder.Name, value);

            public static void SetIndexer(Type type, object[] indexes, object value)
            {
                var methods = ResolveInfoContainer(type).Indexers.Where(x => x.CanWrite)
                                       .Select(x => x.GetSetCaller())
                                       .Where(x => x != null)
                                       .ToArray();

                CallerSelector.SetIndexer((object) null, methods, indexes, value);
            }

            public static bool TrySetIndexer(Type type, object[] indexes, object value)
            {
                var methods = ResolveInfoContainer(type).Indexers.Where(x => x.CanWrite)
                                       .Select(x => x.GetSetCaller())
                                       .Where(x => x != null)
                                       .ToArray();

                return CallerSelector.TrySetIndexer((object) null, methods, indexes, value);
            }

            public static object Call(Type type, InvokeMemberBinder binder, params object[] args) => Call(type, binder.Name, args);
            public static bool TryCall(Type type, InvokeMemberBinder binder, IEnumerable<object> args, out object result) => TryCall(type, binder.Name, args, out result);

            public static object Call(Type type, string methodName, params object[] args)
            {
                var methods = ResolveInfoContainer(type).Methods[methodName];
                return CallerSelector.Call((object)null, methods, args);
            }


            public static bool TryCall(Type type, string methodName, IEnumerable<object> args, out object result)
            {
                var methods = ResolveInfoContainer(type).Methods[methodName];
                return CallerSelector.TryCall((object)null, methods, args, out result);
            }
                       
            public static bool TryRaiseEvent(Type type, string @event, params dynamic[] arguments)
            {
                EventCaller caller;
                if (!ResolveInfoContainer(type).Events.TryGetValue(@event, out caller))
                    return false;

                caller.Raise((object) null, arguments);
                return true;
            }

            public static bool TryAddEventHandler(Type type, string @event, params Delegate[] delegates)
            {
                EventCaller caller;
                if (!ResolveInfoContainer(type).Events.TryGetValue(@event, out caller))
                    return false;

                caller.Add((object) null, delegates);
                return true;
            }

            public static bool TryRemoveEventHandler(Type type, string @event, params Delegate[] delegates)
            {
                EventCaller caller;
                if (!ResolveInfoContainer(type).Events.TryGetValue(@event, out caller))
                    return false;

                caller.Remove((object) null, delegates);
                return true;
            }


            



            public static bool HasField(Type type, string field) => HasGetterField(type, field); //No need to check setter

            public static bool HasProperty(Type type, string property) => HasGetterProperty(type, property) || HasSetterProperty(type, property);

            public static bool HasGetterProperty(Type type, string property) => ResolveInfoContainer(type).Properties.ContainsKey(property);

            public static bool HasSetterProperty(Type type, string property) => ResolveInfoContainer(type).Properties.ContainsKey(property);

            public static bool HasGetterField(Type type, string field) => ResolveInfoContainer(type).Fields.ContainsKey(field);

            public static bool HasSetterField(Type type, string field) => ResolveInfoContainer(type).Fields.ContainsKey(field);

            public static bool HasMethod(Type type, string method) => ResolveInfoContainer(type).Methods.Contains(method);

            public static IEnumerable<IMethodCaller> GetMethod(Type type, string method) => ResolveInfoContainer(type).Methods[method];

            public static bool HasEvent(Type type, string @event) => ResolveInfoContainer(type).Events.ContainsKey(@event);

            public static IEventCaller GetEvent(Type type, string @event) => ResolveInfoContainer(type).Events[@event];

            public static IMethodCaller GetSpecificMethod(Type type, string method, params Type[] arguments) => Info.Extended.ResolveSpecificCaller(ResolveInfoContainer(type).Methods[method], arguments);
        }
    }
#pragma warning restore S3218 // Inner class members should not shadow outer class "static" or type members
}
