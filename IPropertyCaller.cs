using System.Reflection;

namespace Horizon
{
    partial interface IPropertyCaller : IMemberCaller
    {
        MethodInfo GetSetMethod();
        MethodInfo GetGetMethod();

        IMethodCaller GetSetCaller();
        IMethodCaller GetGetCaller();

        PropertyInfo PropertyInfo { get; }

        bool IsIndexer { get; }
    }

    partial interface IPropertyCaller<in T> : IMemberCaller<T>, IPropertyCaller
    {

    }
}