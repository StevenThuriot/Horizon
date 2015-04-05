using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Horizon
{
    [DebuggerDisplay("{GetType().Name} - {Info.ToString()}")]
    class MethodCaller : IInternalCaller
	{
        public static MethodCaller Create(MethodInfo info)
        {
            if (info.IsGenericMethodDefinition)
                return new GenericMethodCaller(info);

            return new MethodCaller(info);
        }


        public string Name { get; private set; }

	    public IReadOnlyList<SimpleParameterInfo> ParameterTypes { get; private set; }
	    private readonly Lazy<Delegate> _caller;
        private readonly Lazy<bool> _isAsync;

        protected readonly MethodInfo Info;

        internal MethodCaller(MethodInfo info)
            : this(info, info.GetParameters().Select(x =>new SimpleParameterInfo(x)))
        {
            _caller = info.BuildLazy();
        }

        protected MethodCaller(MethodInfo info, IEnumerable<SimpleParameterInfo> parameterTypes)
        {
            Info = info;
            Name = info.Name;
			ParameterTypes = parameterTypes.ToArray();
            _isAsync = new Lazy<bool>(() => typeof(Task).IsAssignableFrom(info.ReturnType));
        }

        public bool IsAsync
        {
            get { return _isAsync.Value; }
        }

        public bool IsStatic
        {
            get { return Info.IsStatic; }
        }

        public Type ReturnType
        {
            get { return Info.ReturnType; }
        }


        public override bool Equals(object obj)
        {
            if (Reference.IsNull(obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MethodCaller) obj);
        }

        protected bool Equals(MethodCaller other)
        {
            return string.Equals(Name, other.Name) && Equals(ParameterTypes, other.ParameterTypes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ ParameterTypes.GetHashCode();
            }
        }


        public virtual object Call(IEnumerable<dynamic> values)
        {
            var arguments = values.ToArray();
            return _caller.Value.FastInvoke(arguments);
        }
    }
}