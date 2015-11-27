using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Horizon
{
    class ConstructorCaller : IConstructorCaller
    {
        readonly Lazy<Delegate> _caller;
        readonly ConstructorInfo _info;

        public ConstructorInfo ConstructorInfo { get { return _info; } }

        public string Name { get; private set; }
        

        internal ConstructorCaller(ConstructorInfo info)
            : this(info, info.GetParameters().Select(x => new SimpleParameterInfo(x)))
        {
            _caller = info.BuildLazy();
        }

        protected ConstructorCaller(ConstructorInfo info, IEnumerable<SimpleParameterInfo> parameterTypes)
        {
            _info = info;
            Name = info.Name;
            ParameterTypes = parameterTypes.ToArray();
        }

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes { get; private set; }

        public bool IsStatic => _info.IsStatic;

        public virtual object Call(IEnumerable<dynamic> values)
        {
            var arguments = values.ToArray();
            return _caller.Value.FastInvoke(arguments);
        }


        public override bool Equals(object obj)
        {
            if (Reference.IsNull(obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((ConstructorCaller) obj);
        }

        protected bool Equals(ConstructorCaller other) => string.Equals(Name, other.Name) && Equals(ParameterTypes, other.ParameterTypes);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ ParameterTypes.GetHashCode();
            }
        }
    }
}