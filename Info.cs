using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Horizon
{
    static partial class Info<T>
    {
#pragma warning disable S2743 // Static fields should not be used in generic types

        static readonly ILookup<string, MethodCaller> _methods;

        static readonly List<ConstructorCaller> _constructors = new List<ConstructorCaller>();
        
        static readonly Dictionary<string, EventCaller> _events = new Dictionary<string, EventCaller>();

        static readonly Dictionary<string, PropertyCaller<T>> _properties = new Dictionary<string, PropertyCaller<T>>();
        static readonly Dictionary<string, MemberCaller<T>> _fields = new Dictionary<string, MemberCaller<T>>();

#pragma warning restore S2743 // Static fields should not be used in generic types

        static Info()
        {
            var methods = new List<MethodCaller>();
            var events = new List<EventCaller>();

            foreach (var member in typeof(T).GetMembers(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static //default getMember flags
                                                                                | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)) //Our additional flags
            {
                var key = member.Name;

                if ((MemberTypes.Property & member.MemberType) == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo) member;
                    _properties[key] = new PropertyCaller<T>(propertyInfo);
                }
                else if ((MemberTypes.Field & member.MemberType) == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo)member;
                    _fields[key] = new MemberCaller<T>(fieldInfo);
                }
                else if ((MemberTypes.Method & member.MemberType) == MemberTypes.Method)
                {
                    var methodInfo = (MethodInfo) member;
                    if (!methodInfo.IsSpecialName)
                    {
                        //Skip Properties, events, ... 
                        var caller = MethodCaller.Create(methodInfo);
                        methods.Add(caller);
                    }
                }
				else if ((MemberTypes.Constructor & member.MemberType) == MemberTypes.Constructor)
				{
					var constructorInfo = (ConstructorInfo)member;
                    var caller = new ConstructorCaller(constructorInfo);
					_constructors.Add(caller);
				}
                else if ((MemberTypes.Event & member.MemberType) == MemberTypes.Event)
                {
                    var eventInfo = (EventInfo) member;
                    var caller = new EventCaller(eventInfo);

                    events.Add(caller);
                }
			}
            
            _methods = methods.OrderBy(x => x is GenericMethodCaller)//this will make sure non-generic caller are prefered.
                              .ToLookup(x => x.Name, x => x);

            _events = events.ToDictionary(x => x.Name);
        }

        public static object GetProperty(T instance, string property) => _properties[property].Get(instance);

        public static object GetField(T instance, string field) => _fields[field].Get(instance);

        public static void SetProperty(T instance, string property, object value) => _properties[property].Set(instance, value);

        public static void SetField(T instance, string field, object value) => _fields[field].Set(instance, value);

        public static void RaiseEvent(T instance, string @event, params dynamic[] arguments) => _events[@event].Raise(instance, arguments);
        
        public static void AddEventHandler(T instance, string @event, params Delegate[] delegates) => _events[@event].Add(instance, delegates); 

        public static void RemoveEventHandler(T instance, string @event, params Delegate[] delegates) => _events[@event].Remove(instance, delegates); 



        public static object GetValue(T instance, string propertyOrField)
        {
            PropertyCaller<T> getter;
            if (_properties.TryGetValue(propertyOrField, out getter))
                return getter.Get(instance);

            return _fields[propertyOrField].Get(instance);
        }


        public static bool TryGetValue(T instance, string propertyOrField, out object result)
        {
            PropertyCaller<T> prop;
            if (_properties.TryGetValue(propertyOrField, out prop))
                return prop.TryGet(instance, out result);

            MemberCaller<T> field;
            if (_fields.TryGetValue(propertyOrField, out field))
                return field.TryGet(instance, out result);

            result = null;
            return false;
        }


        public static void SetValue(T instance, string propertyOrField, object value)
        {
            PropertyCaller<T> prop;
            if (_properties.TryGetValue(propertyOrField, out prop))
            {
                prop.Set(instance, value);
            }
            else
            {
                _fields[propertyOrField].Set(instance, value);
            }
        }


        public static bool TrySetValue(T instance, string propertyOrField, object value)
        {
            PropertyCaller<T> prop;
            if (_properties.TryGetValue(propertyOrField, out prop))
                return prop.TrySet(instance, value);

            MemberCaller<T> field;
            if (_fields.TryGetValue(propertyOrField, out field))
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
            if (_properties.TryGetValue(property, out getter))
                return getter.TryGet(instance, out result);

            result = null;
            return false;
        }
        
        public static bool TryGetField(T instance, string field, out object result)
        {
            MemberCaller<T> getter;
            if (_fields.TryGetValue(field, out getter))
                return getter.TryGet(instance, out result);

            result = null;
            return false;
        }

        public static object GetProperty(T instance, GetMemberBinder binder) => GetProperty(instance, binder.Name);
        public static bool TryGetProperty(T instance, GetMemberBinder binder, out object result) => TryGetProperty(instance, binder.Name, out result);

        public static object GetIndexer(T instance, object[] indexes)
        {
            var methods = _properties.Values.Where(x => x.CanRead && x.Indexer)
                                            .Select(x => x.GetGetCaller())
                                            .ToArray();

            return CallerSelector.GetIndexer(instance, methods, indexes);
        }

        public static bool TryGetIndexer(T instance, object[] indexes, out object result)
        {
            var methods = _properties.Values.Where(x => x.CanRead && x.Indexer)
                                            .Select(x => x.GetGetCaller())
                                            .ToArray();

            return CallerSelector.TryGetIndexer(instance, methods, indexes, out result);
        }

        public static bool TrySetProperty(T instance, string property, object value)
        {
            PropertyCaller<T> setter;
            return _properties.TryGetValue(property, out setter) && setter.TrySet(instance, value);
        }

        public static void SetProperty(T instance, SetMemberBinder binder, object value) => SetProperty(instance, binder.Name, value);
        public static bool TrySetProperty(T instance, SetMemberBinder binder, object value) => TrySetProperty(instance, binder.Name, value);

        public static bool TrySetField(T instance, string field, object value)
        {
            MemberCaller<T> setter;
            return _fields.TryGetValue(field, out setter) && setter.TrySet(instance, value);
        }

        public static void SetField(T instance, SetMemberBinder binder, object value) => SetField(instance, binder.Name, value);
        public static bool TrySetField(T instance, SetMemberBinder binder, object value) => TrySetField(instance, binder.Name, value);

        public static void SetIndexer(T instance, object[] indexes, object value)
        {
            var methods = _properties.Values.Where(x => x.CanWrite && x.Indexer)
                                            .Select(x => x.GetSetCaller())
                                            .ToArray();

            CallerSelector.SetIndexer(instance, methods, indexes, value);
        }

        public static bool TrySetIndexer(T instance, object[] indexes, object value)
        {
            var methods = _properties.Values.Where(x => x.CanWrite && x.Indexer)
                                            .Select(x => x.GetSetCaller())
                                            .ToArray();

            return CallerSelector.TrySetIndexer(instance, methods, indexes, value);
        }

        public static object Call(T instance, InvokeMemberBinder binder, IEnumerable<object> args) => Call(instance, binder.Name, args);
        public static object Call(InvokeMemberBinder binder, IEnumerable<object> args) => Call(binder.Name, args);
        public static bool TryCall(T instance, InvokeMemberBinder binder, IEnumerable<object> args, out object result) => TryCall(instance, binder.Name, args, out result);
        public static bool TryCall(InvokeMemberBinder binder, IEnumerable<object> args, out object result) => TryCall(binder.Name, args, out result);

        public static object Call(T instance, string methodName, IEnumerable<object> args)
        {
            var methods = _methods[methodName];
            return CallerSelector.Call(instance, args, methods);
        }

        public static object Call(string methodName, IEnumerable<object> args)
        {
            var methods = _methods[methodName];
            return CallerSelector.Call(args, methods);
        }

        public static bool TryCall(T instance, string methodName, IEnumerable<object> args, out object result)
        {
            var methods = _methods[methodName];
            return CallerSelector.TryCall(instance, args, methods, out result);
        }

        public static bool TryCall(string methodName, IEnumerable<object> args, out object result)
        {
            var methods = _methods[methodName];
            return CallerSelector.TryCall(args, methods, out result);
        }

        public static T Create(params object[] args) => (T)CallerSelector.Create(_constructors, args);

        public static bool TryCreate(out T instance, params object[] args)
	    {
		    object boxedInstance;
		    if (CallerSelector.TryCreate(_constructors, args, out boxedInstance))
		    {
			    instance = (T) boxedInstance;
			    return true;
		    }

		    instance = default(T);
		    return false;
	    }

        public static bool TryRaiseEvent(T instance, string @event, params dynamic[] arguments)
        {
            EventCaller caller;
            if (!_events.TryGetValue(@event, out caller))
                return false;

            caller.Raise(instance, arguments);
            return true;
        }

        public static bool TryAddEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            EventCaller caller;
            if (!_events.TryGetValue(@event, out caller))
                return false;

            caller.Add(instance, delegates);
            return true;
        }

        public static bool TryRemoveEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            EventCaller caller;
            if (!_events.TryGetValue(@event, out caller))
                return false;

            caller.Remove(instance, delegates);
            return true;
        }







        public static bool HasField(string field) => HasGetterField(field); //No need to check setter

        public static bool HasProperty(string property) => HasGetterProperty(property) || HasSetterProperty(property);

        public static bool HasGetterProperty(string property) => _properties.ContainsKey(property);

        public static bool HasSetterProperty(string property) => _properties.ContainsKey(property);

        public static bool HasGetterField(string field) => _fields.ContainsKey(field);

        public static bool HasSetterField(string field) => _fields.ContainsKey(field);

        public static bool HasMethod(string method) => _methods.Contains(method);

        public static IEnumerable<IMethodCaller> GetMethod(string method) => _methods[method];

        public static bool HasEvent(string @event) => _events.ContainsKey(@event);

        public static IEventCaller GetEvent(string @event) => _events[@event];

        public static IMethodCaller GetSpecificMethod(string method, params Type[] arguments) => ResolveSpecificCaller(_methods[method], arguments);

        public static IConstructorCaller GetConstructor(params Type[] arguments) => ResolveSpecificCaller(_constructors, arguments);


        //Simple specific version of the caller selector which enforces types and argument count.
        static TCaller ResolveSpecificCaller<TCaller>(IEnumerable<TCaller> callers, IReadOnlyList<Type> arguments)
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

            throw new ArgumentException("No matching caller found.");
        }








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
            var methods = _methods["op_Implicit"];
            var owner = typeof(T);
            var method = methods.FirstOrDefault(x => x.ReturnType == type && x.ParameterTypes[0].ParameterType == owner);

            return method;
        }
    }
}