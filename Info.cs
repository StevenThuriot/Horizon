using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Horizon
{
    static partial class Info<T>
    {
        static InfoContainer<T> container = new InfoContainer<T>();

        public static object GetProperty(T instance, string property) => container.Properties[property].Get(instance);

        public static object GetField(T instance, string field) => container.Fields[field].Get(instance);

        public static void SetProperty(T instance, string property, object value) => container.Properties[property].Set(instance, value);

        public static void SetField(T instance, string field, object value) => container.Fields[field].Set(instance, value);

        public static void RaiseEvent(T instance, string @event, params dynamic[] arguments) => container.Events[@event].Raise(instance, arguments);

        public static void AddEventHandler(T instance, string @event, params Delegate[] delegates) => container.Events[@event].Add(instance, delegates);

        public static void RemoveEventHandler(T instance, string @event, params Delegate[] delegates) => container.Events[@event].Remove(instance, delegates);

        public static object GetValue(T instance, string propertyOrField)
        {
            PropertyCaller<T> getter;
            if (container.Properties.TryGetValue(propertyOrField, out getter))
                return getter.Get(instance);

            return container.Fields[propertyOrField].Get(instance);
        }


        public static bool TryGetValue(T instance, string propertyOrField, out object result)
        {
            PropertyCaller<T> prop;
            if (container.Properties.TryGetValue(propertyOrField, out prop))
                return prop.TryGet(instance, out result);

            MemberCaller<T> field;
            if (container.Fields.TryGetValue(propertyOrField, out field))
                return field.TryGet(instance, out result);

            result = null;
            return false;
        }


        public static void SetValue(T instance, string propertyOrField, object value)
        {
            PropertyCaller<T> prop;
            if (container.Properties.TryGetValue(propertyOrField, out prop))
            {
                prop.Set(instance, value);
            }
            else
            {
                container.Fields[propertyOrField].Set(instance, value);
            }
        }


        public static bool TrySetValue(T instance, string propertyOrField, object value)
        {
            PropertyCaller<T> prop;
            if (container.Properties.TryGetValue(propertyOrField, out prop))
                return prop.TrySet(instance, value);

            MemberCaller<T> field;
            if (container.Fields.TryGetValue(propertyOrField, out field))
                return field.TrySet(instance, value);

            return false;
        }


        public static object GetValue(T instance, GetMemberBinder binder) => GetValue(instance, binder.Name);
        public static bool TryGetValue(T instance, GetMemberBinder binder, out object result) => TryGetValue(instance, binder.Name, out result);

        public static void SetValue(T instance, SetMemberBinder binder, object value) => SetValue(instance, binder.Name, value);
        public static bool TrySetValue(T instance, SetMemberBinder binder, object value) => TrySetValue(instance, binder.Name, value);


        public static bool TryGetProperty(T instance, string property, out object result)
        {
            PropertyCaller<T> getter;
            if (container.Properties.TryGetValue(property, out getter))
                return getter.TryGet(instance, out result);

            result = null;
            return false;
        }

        public static bool TryGetField(T instance, string field, out object result)
        {
            MemberCaller<T> getter;
            if (container.Fields.TryGetValue(field, out getter))
                return getter.TryGet(instance, out result);

            result = null;
            return false;
        }

        public static object GetProperty(T instance, GetMemberBinder binder) => GetProperty(instance, binder.Name);
        public static bool TryGetProperty(T instance, GetMemberBinder binder, out object result) => TryGetProperty(instance, binder.Name, out result);

        public static object GetIndexer(T instance, object[] indexes)
        {
            var methods = container.Indexers.Where(x => x.CanRead)
                                   .Select(x => x.GetGetCaller())
                                   .ToArray();

            return CallerSelector.GetIndexer(instance, methods, indexes);
        }

        public static bool TryGetIndexer(T instance, object[] indexes, out object result)
        {
            var methods = container.Indexers.Where(x => x.CanRead)
                                   .Select(x => x.GetGetCaller())
                                   .Where(x => x != null)
                                   .ToArray();

            return CallerSelector.TryGetIndexer(instance, methods, indexes, out result);
        }

        public static bool TrySetProperty(T instance, string property, object value)
        {
            PropertyCaller<T> setter;
            return container.Properties.TryGetValue(property, out setter) && setter.TrySet(instance, value);
        }

        public static void SetProperty(T instance, SetMemberBinder binder, object value) => SetProperty(instance, binder.Name, value);
        public static bool TrySetProperty(T instance, SetMemberBinder binder, object value) => TrySetProperty(instance, binder.Name, value);

        public static bool TrySetField(T instance, string field, object value)
        {
            MemberCaller<T> setter;
            return container.Fields.TryGetValue(field, out setter) && setter.TrySet(instance, value);
        }

        public static void SetField(T instance, SetMemberBinder binder, object value) => SetField(instance, binder.Name, value);
        public static bool TrySetField(T instance, SetMemberBinder binder, object value) => TrySetField(instance, binder.Name, value);

        public static void SetIndexer(T instance, object[] indexes, object value)
        {
            var methods = container.Indexers.Where(x => x.CanWrite)
                                   .Select(x => x.GetSetCaller())
                                   .Where(x => x != null)
                                   .ToArray();

            CallerSelector.SetIndexer(instance, methods, indexes, value);
        }

        public static bool TrySetIndexer(T instance, object[] indexes, object value)
        {
            var methods = container.Indexers.Where(x => x.CanWrite)
                                   .Select(x => x.GetSetCaller())
                                   .Where(x => x != null)
                                   .ToArray();

            return CallerSelector.TrySetIndexer(instance, methods, indexes, value);
        }

        public static object Call(T instance, InvokeMemberBinder binder, params object[] args) => Call(instance, binder.Name, args);
        public static object Call(InvokeMemberBinder binder, params object[] args) => Call(binder.Name, args);
        public static bool TryCall(T instance, InvokeMemberBinder binder, IEnumerable<object> args, out object result) => TryCall(instance, binder.Name, args, out result);
        public static bool TryCall(InvokeMemberBinder binder, IEnumerable<object> args, out object result) => TryCall(binder.Name, args, out result);

        public static object Call(T instance, string methodName, params object[] args)
        {
            var methods = container.Methods[methodName];
            return CallerSelector.Call(instance, methods, args);
        }

        public static object Call(string methodName, params object[] args)
        {
            var methods = container.Methods[methodName];
            return CallerSelector.Call(methods, args);
        }

        public static bool TryCall(T instance, string methodName, IEnumerable<object> args, out object result)
        {
            var methods = container.Methods[methodName];
            return CallerSelector.TryCall(instance, methods, args, out result);
        }

        public static bool TryCall(string methodName, IEnumerable<object> args, out object result)
        {
            var methods = container.Methods[methodName];
            return CallerSelector.TryCall(methods, args, out result);
        }

        public static T Create(params object[] args) => (T)CallerSelector.Create(container.Constructors, args);

        public static bool TryCreate(out T instance, params object[] args)
        {
            object boxedInstance;
            if (CallerSelector.TryCreate(container.Constructors, args, out boxedInstance))
            {
                instance = (T)boxedInstance;
                return true;
            }

            instance = default(T);
            return false;
        }

        public static bool TryRaiseEvent(T instance, string @event, params object[] arguments)
        {
            EventCaller caller;
            if (!container.Events.TryGetValue(@event, out caller))
                return false;

            caller.Raise(instance, arguments);
            return true;
        }

        public static bool TryAddEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            EventCaller caller;
            if (!container.Events.TryGetValue(@event, out caller))
                return false;

            caller.Add(instance, delegates);
            return true;
        }

        public static bool TryRemoveEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            EventCaller caller;
            if (!container.Events.TryGetValue(@event, out caller))
                return false;

            caller.Remove(instance, delegates);
            return true;
        }







        public static bool HasField(string field) => HasGetterField(field); //No need to check setter

        public static bool HasProperty(string property) => HasGetterProperty(property) || HasSetterProperty(property);

        public static bool HasGetterProperty(string property)
        {
            PropertyCaller<T> caller;
            if (container.Properties.TryGetValue(property, out caller))
            {
                return caller.CanRead;
            }

            return false;
        }

        public static bool HasSetterProperty(string property)
        {
            PropertyCaller<T> caller;
            if (container.Properties.TryGetValue(property, out caller))
            {
                return caller.CanWrite;
            }

            return false;
        }

        public static bool HasGetterField(string field)
        {
            MemberCaller<T> caller;
            if (container.Fields.TryGetValue(field, out caller))
            {
                return caller.CanRead;
            }

            return false;
        }

        public static bool HasSetterField(string field)
        {
            MemberCaller<T> caller;
            if (container.Fields.TryGetValue(field, out caller))
            {
                return caller.CanWrite;
            }

            return false;
        }

        public static bool HasMethod(string method) => container.Methods.Contains(method);

        public static IEnumerable<IMethodCaller> GetMethod(string method) => container.Methods[method];

        public static bool HasEvent(string @event) => container.Events.ContainsKey(@event);

        public static IEventCaller GetEvent(string @event) => container.Events[@event];

        public static IMethodCaller GetSpecificMethod(string method, params Type[] arguments) => Info.Extended.ResolveSpecificCaller(container.Methods[method], arguments);

        public static IConstructorCaller GetConstructor(params Type[] arguments) => Info.Extended.ResolveSpecificCaller(container.Constructors, arguments);









        public static bool CanImplicitConvert(T instance, Type type)
        {
            var method = Findop_Implicit(type);
            return method != null;
        }

        public static dynamic ImplicitConvert(T instance, Type type)
        {
            var method = Findop_Implicit(type);

            if (method == null)
                throw new ArgumentException("Invalid implicit conversion");

            var arguments = new dynamic[] { instance };
            return method.Call(arguments);
        }

        public static bool TryImplicitConvert(T instance, Type type, out dynamic result)
        {
            var method = Findop_Implicit(type);

            if (method == null)
            {
                result = null;
                return false;
            }

            var arguments = new dynamic[] { instance };

            result = method.Call(arguments);
            return true;
        }

        static MethodCaller Findop_Implicit(Type type)
        {
            var methods = container.Methods["op_Implicit"];
            var owner = typeof(T);
            var method = methods.FirstOrDefault(x => x.ReturnType == type && x.ParameterTypes[0].ParameterType == owner);

            return method;
        }
    }
}