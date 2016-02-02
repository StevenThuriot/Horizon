using System;
using System.Reflection;

namespace Horizon
{
    class PropertyCaller<T> : MemberCaller<T>, IPropertyCaller<T>
    {
        readonly Lazy<MethodCaller> _getCaller;
        readonly Lazy<MethodCaller> _setCaller;

        public PropertyCaller(PropertyInfo info) : base(info)
        {
            IsIndexer = info.GetIndexParameters().Length != 0;  //info.GetCustomAttribute<IndexerNameAttribute>() != null;

            _getCaller = new Lazy<MethodCaller>(() =>
            {
                var method = GetGetMethod();

                if (method == null)
                    return null;

                return new MethodCaller(method);
            });

            _setCaller = new Lazy<MethodCaller>(() =>
            {
                var method = GetSetMethod();

                if (method == null)
                    return null;

                return new MethodCaller(method);
            });
        }

        public bool IsIndexer { get; }

        public MethodInfo GetSetMethod() => PropertyInfo.GetSetMethod();

        public MethodInfo GetGetMethod() => PropertyInfo.GetGetMethod();

        public PropertyInfo PropertyInfo => (PropertyInfo)_info;

        public MethodCaller GetSetCaller() => _setCaller.Value;

        public MethodCaller GetGetCaller() => _getCaller.Value;

    }
}