using System.Reflection;
using System.Runtime.CompilerServices;

namespace Horizon
{
    class PropertyCaller<T> : MemberCaller<T>, IPropertyCaller<T>
    {
        public PropertyCaller(PropertyInfo info) : base(info)
        {
            Indexer = info.GetCustomAttribute<IndexerNameAttribute>() != null;
        }

        public MethodInfo GetSetMethod() => PropertyInfo.GetSetMethod();

        public MethodInfo GetGetMethod() => PropertyInfo.GetGetMethod();
        
        public PropertyInfo PropertyInfo => (PropertyInfo)_info;

        public bool Indexer { get; }

        //TODO : Cache
        public MethodCaller GetSetCaller() => new MethodCaller(PropertyInfo.GetSetMethod());

        public MethodCaller GetGetCaller() => new MethodCaller(PropertyInfo.GetGetMethod());

    }
}