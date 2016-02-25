using System.Reflection;

namespace Horizon
{
    partial interface IPropertyCaller : IMemberCaller
    {
        MethodInfo GetSetMethod();
        MethodInfo GetGetMethod();

        MethodCaller GetSetCaller();
        MethodCaller GetGetCaller();

        PropertyInfo PropertyInfo { get; }

        bool IsIndexer { get; }
    }

    partial interface IPropertyCaller<in T> : IMemberCaller<T>, IPropertyCaller
    {

    }
}