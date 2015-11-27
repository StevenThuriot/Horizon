using System.Reflection;

namespace Horizon
{
    class PropertyCaller<T> : MemberCaller<T>, IPropertyCaller<T>
    {
        public PropertyCaller(PropertyInfo info) : base(info)
        {
        }

        public MethodInfo GetSetMethod() => PropertyInfo.GetSetMethod();

        public MethodInfo GetGetMethod() => PropertyInfo.GetGetMethod();

        public PropertyInfo PropertyInfo => (PropertyInfo)_info;
    }
}