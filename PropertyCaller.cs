using System.Reflection;

namespace Horizon
{
    class PropertyCaller<T> : MemberCaller<T>, IPropertyCaller<T>
    {
        private readonly PropertyInfo _info;

        public PropertyCaller(PropertyInfo info) : base(info)
        {
            _info = info;
        }

        public MethodInfo GetSetMethod()
        {
            return _info.GetSetMethod();
        }

        public MethodInfo GetGetMethod()
        {
            return _info.GetGetMethod();
        }
    }
}