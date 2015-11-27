using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Horizon
{
    class GenericMethodCaller : MethodCaller
    {
        readonly Dictionary<TypeHash, Delegate> _cache = new Dictionary<TypeHash, Delegate>();

        internal GenericMethodCaller(MethodInfo info)
            : base(info.GetGenericMethodDefinition(), GetParameters(info))
        {
        }


        static IEnumerable<SimpleParameterInfo> GetParameters(MethodInfo info)
        {
            foreach (var parameterInfo in info.GetParameters())
            {
                var name = parameterInfo.Name;
                var hasDefaultValue = parameterInfo.HasDefaultValue;
                var defaultValue = hasDefaultValue ? parameterInfo.DefaultValue : null;

                var type = parameterInfo.ParameterType;

                var genericTypeDefinition = type;

                if (!type.IsGenericParameter && type.ContainsGenericParameters)
                    genericTypeDefinition = type.GetGenericTypeDefinition();

                yield return new SimpleParameterInfo(name, defaultValue, genericTypeDefinition, type, hasDefaultValue);
            }
        }

        public override object Call(IEnumerable<dynamic> values)
        {
            var arguments = values.ToArray();
            var objectType = typeof (object);

            var types = arguments.Cast<object>()
                                 .Select(x => Reference.IsNull(x) ? objectType : x.GetType());

            var hash = new TypeHash(types);

            Delegate @delegate;
            if (!_cache.TryGetValue(hash, out @delegate))
            {
                @delegate = _info.BuildCallSite(arguments);
                _cache[hash] = @delegate;
            }

            return @delegate.FastInvoke(arguments);
        }
    }
}