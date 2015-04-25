using System.Reflection;

namespace Horizon
{
    class PropertyCaller<T> : MemberCaller<T>, IPropertyCaller<T>
    {
        public PropertyCaller(PropertyInfo info) : base(info)
        {
        }

        public MethodInfo GetSetMethod()
        {
            return PropertyInfo.GetSetMethod();
        }

        public MethodInfo GetGetMethod()
        {
            return PropertyInfo.GetGetMethod();
        }

        public PropertyInfo PropertyInfo { get { return (PropertyInfo) _info; } }
    }
}