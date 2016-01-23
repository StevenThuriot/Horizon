using System.Reflection;
using System.Runtime.CompilerServices;

namespace Horizon
{
    class PropertyCaller<T> : MemberCaller<T>, IPropertyCaller<T>
    {
        public PropertyCaller(PropertyInfo info) : base(info)
        {
            Indexer = info.GetIndexParameters().Length != 0;  //info.GetCustomAttribute<IndexerNameAttribute>() != null;
        }

        public MethodInfo GetSetMethod() => PropertyInfo.GetSetMethod();

        public MethodInfo GetGetMethod() => PropertyInfo.GetGetMethod();

        public PropertyInfo PropertyInfo => (PropertyInfo)_info;

        public bool Indexer { get; }

        //TODO : Cache
        public MethodCaller GetSetCaller()
        {
            var method = GetSetMethod();

            if (method == null)
                return null;

            return new MethodCaller(method);
        }

        public MethodCaller GetGetCaller()
        {
            var method = GetGetMethod();

            if (method == null)
                return null;

            return new MethodCaller(method);
        }

    }
}