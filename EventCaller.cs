using System.Collections.Generic;
using System.Reflection;

namespace Horizon
{
    class EventCaller : IInternalCaller, IEventCaller
    {
        private readonly EventInfo _info;

        private readonly IInternalCaller _raise;
        private readonly ICaller _add;
        private readonly ICaller _remove;

        public EventCaller(EventInfo info)
        {
            _info = info;

            var raise = info.RaiseMethod;
            _raise = raise == null ? NullCaller.Instance : new MethodCaller(raise);

            _add = new MethodCaller(info.AddMethod);
            _remove = new MethodCaller(info.RemoveMethod);
        }

        public void Add(IEnumerable<dynamic> values)
        {
            _add.Call(values);
        }

        public void Remove(IEnumerable<dynamic> values)
        {
            _remove.Call(values);
        }

        public string Name
        {
            get { return _info.Name; }
        }

        public object Call(IEnumerable<dynamic> values)
        {
            return _raise.Call(values);
        }

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes
        {
            get { return _raise.ParameterTypes; }
        }

        public bool IsStatic
        {
            get { return false; }
        }
    }
}