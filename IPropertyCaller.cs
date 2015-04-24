using System.Reflection;

namespace Horizon
{
    partial interface IPropertyCaller : IMemberCaller
    {
        MethodInfo GetSetMethod();
        MethodInfo GetGetMethod();
    }

    partial interface IPropertyCaller<in T> : IMemberCaller<T>, IPropertyCaller
    {

    }
}