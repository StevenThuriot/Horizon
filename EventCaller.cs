using System;
using System.Collections.Generic;
using System.Reflection;

namespace Horizon
{
    class EventCaller : IEventCaller
    {
        readonly EventInfo _info;
        readonly Lazy<bool> _isStatic;

        readonly IMethodCaller _raise;
        readonly ICaller _add;
        readonly ICaller _remove;

        public bool CanRaise { get; }
        public Type EventHandlerType => _info.EventHandlerType;

        public EventCaller(EventInfo info)
        {
            _info = info;
            _isStatic = new Lazy<bool>(() => _info.AddMethod.IsStatic);

            var raise = info.RaiseMethod;
            CanRaise = raise != null;
            _raise = CanRaise ? new MethodCaller(raise) : NullCaller.Instance;

            _add = new MethodCaller(info.AddMethod);
            _remove = new MethodCaller(info.RemoveMethod);
        }

        dynamic[] BuildCallerArguments(dynamic instance, dynamic[] arguments)
        {
            var length = arguments.Length;

            if (!IsStatic)
                length += 1;

            var values = new dynamic[length];

            if (!IsStatic)
                values[0] = instance;

            if (length != 0)
                Array.Copy(arguments, 0, values, 1, length);

            return values;
        }

        public object Call(IEnumerable<dynamic> values) => _raise.Call(values);

        public void Raise(dynamic instance, params dynamic[] arguments)
        {
            dynamic[] values = BuildCallerArguments(instance, arguments);
            _raise.Call(values);
        }

        public void Add(dynamic instance, params Delegate[] handlers)
        {
            dynamic[] values = BuildCallerArguments(instance, handlers);
            _add.Call(values);
        }

        public void Remove(dynamic instance, params Delegate[] handlers)
        {
            dynamic[] values = BuildCallerArguments(instance, handlers);
            _remove.Call(values);
        }

        public string Name => _info.Name;

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes => _raise.ParameterTypes;
        
        public bool IsStatic => _isStatic.Value;
    }
}