using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Horizon
{
    static partial class TypeInfo<T>
    {
        private static readonly ILookup<string, MethodCaller> _methods;

	    private static readonly List<ConstructorCaller> _constructors = new List<ConstructorCaller>();

        private static readonly Dictionary<string, Lazy<Func<T, object>>> _getFields = new Dictionary<string, Lazy<Func<T, object>>>();

        private static readonly Dictionary<string, Lazy<Func<T, object>>> _getProperties = new Dictionary<string, Lazy<Func<T, object>>>();

        private static readonly Dictionary<string, Lazy<Action<T, object>>> _setFields = new Dictionary<string, Lazy<Action<T, object>>>();

        private static readonly Dictionary<string, Lazy<Action<T, object>>> _setProperties = new Dictionary<string, Lazy<Action<T, object>>>();

        private static readonly Dictionary<string, EventCaller> _events = new Dictionary<string, EventCaller>();
        

        static TypeInfo()
        {
            var methods = new List<MethodCaller>();
            var events = new List<EventCaller>();

            foreach (var member in Constants.Typed<T>.OwnerType.GetMembers())
            {
                var key = member.Name;

                if ((MemberTypes.Property & member.MemberType) == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo) member;
                    if (propertyInfo.CanWrite)
                        _setProperties[key] = InvokeHelper<T>.CreateSetterLazy(propertyInfo);

                    if (propertyInfo.CanRead)
                        _getProperties[key] = InvokeHelper<T>.CreateGetterLazy(propertyInfo);
                }
                else if ((MemberTypes.Field & member.MemberType) == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo) member;

                    if (!fieldInfo.IsInitOnly)
                        _setFields[key] = InvokeHelper<T>.CreateSetterLazy(fieldInfo);

                    _getFields[key] = InvokeHelper<T>.CreateGetterLazy(fieldInfo);
                }
                else if ((MemberTypes.Method & member.MemberType) == MemberTypes.Method)
                {
                    var methodInfo = (MethodInfo) member;
                    var caller = MethodCaller.Create(methodInfo);
                    methods.Add(caller);
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

        public static object GetProperty(T instance, string property)
        {
            return _getProperties[property].Value(instance);
        }

        public static object GetField(T instance, string field)
        {
            return _getFields[field].Value(instance);
        }

        public static void SetProperty(T instance, string property, object value)
        {
            _setProperties[property].Value(instance, value);
        }

        public static void SetField(T instance, string field, object value)
        {
            _setFields[field].Value(instance, value);
        }

        public static void RaiseEvent(T instance, string @event, params dynamic[] arguments)
        {
            _events[@event].Raise(instance, arguments);
        }
        
        public static void AddEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            _events[@event].Add(instance, delegates);
        }

        public static void RemoveEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            _events[@event].Remove(instance, delegates);
        }









        public static bool TryGetProperty(T instance, string property, out object result)
        {
            Lazy<Func<T, object>> getter;
            if (_getProperties.TryGetValue(property, out getter))
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
            if (_getFields.TryGetValue(field, out getter))
            {
                result = getter.Value(instance);
                return true;
            }

            result = null;
            return false;
        }

        public static object GetIndexer(T instance, object[] indexes)
        {
            var methods = _methods["get_Item"];
            return CallerSelector.GetIndexer(instance, methods, indexes);
        }

        public static bool TryGetIndexer(T instance, object[] indexes, out object result)
        {
            var methods = _methods["get_Item"];
            return CallerSelector.TryGetIndexer(instance, methods, indexes, out result);
        }

        public static bool TrySetProperty(T instance, string property, object value)
        {
            Lazy<Action<T, object>> setter;
            if (_setProperties.TryGetValue(property, out setter))
            {
                setter.Value(instance, value);
                return true;
            }

            return false;
        }

        public static bool TrySetField(T instance, string field, object value)
        {
            Lazy<Action<T, object>> setter;
            if (_setFields.TryGetValue(field, out setter))
            {
                setter.Value(instance, value);
                return true;
            }

            return false;
        }

        public static void SetIndexer(T instance, object[] indexes, object value)
        {
            var methods = _methods["set_Item"];
            CallerSelector.SetIndexer(instance, methods, indexes, value);
        }

        public static bool TrySetIndexer(T instance, object[] indexes, object value)
        {
            var methods = _methods["set_Item"];
            return CallerSelector.TrySetIndexer(instance, methods, indexes, value);
        }

        public static object Call(T instance, InvokeMemberBinder binder, IEnumerable<object> args)
        {
            var methods = _methods[binder.Name];
            return CallerSelector.Call(instance, binder, args, methods);
        }

        public static object Call(InvokeMemberBinder binder, IEnumerable<object> args)
        {
            var methods = _methods[binder.Name];
            return CallerSelector.Call(binder, args, methods);
        }

        public static bool TryCall(T instance, InvokeMemberBinder binder, IEnumerable<object> args, out object result)
        {
            var methods = _methods[binder.Name];
            return CallerSelector.TryCall(instance, binder, args, methods, out result);
        }

        public static bool TryCall(InvokeMemberBinder binder, IEnumerable<object> args, out object result)
        {
            var methods = _methods[binder.Name];
            return CallerSelector.TryCall(binder, args, methods, out result);
        }

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

	    public static T Create(params object[] args)
	    {
		    return (T) CallerSelector.Create(_constructors, args);
	    }

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
            if (!_events.TryGetValue(@event, out caller)) return false;

            caller.Raise(instance, arguments);
            return true;
        }

        public static bool TryAddEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            EventCaller caller;
            if (!_events.TryGetValue(@event, out caller)) return false;

            caller.Add(instance, delegates);
            return true;
        }

        public static bool TryRemoveEventHandler(T instance, string @event, params Delegate[] delegates)
        {
            EventCaller caller;
            if (!_events.TryGetValue(@event, out caller)) return false;

            caller.Remove(instance, delegates);
            return true;
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
            return _getProperties.ContainsKey(property);
        }

        public static bool HasSetterProperty(string property)
        {
            return _setProperties.ContainsKey(property);
        }

        public static bool HasGetterField(string field)
        {
            return _getFields.ContainsKey(field);
        }

        public static bool HasSetterField(string field)
        {
            return _setFields.ContainsKey(field);
        }

        public static bool HasMethod(string method)
        {
            return _methods.Contains(method);
        }
        
        public static IEnumerable<ICaller> GetMethod(string method)
        {
            return _methods[method];
        }

        public static bool HasEvent(string @event)
        {
            return _events.ContainsKey(@event);
        }
        
        public static IEventCaller GetEvent(string @event)
        {
            return _events[@event];
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

        private static MethodCaller Findop_Implicit(Type type)
        {
            var methods = _methods["op_Implicit"];
            var owner = Constants.Typed<T>.OwnerType;
            var method = methods.FirstOrDefault(x => x.ReturnType == type && x.ParameterTypes[0].ParameterType == owner);

            return method;
        }
    }
}