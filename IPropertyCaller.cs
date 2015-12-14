using System.Reflection;

namespace Horizon
{
    partial interface IPropertyCaller : IMemberCaller
    {
        MethodInfo GetSetMethod();
        MethodInfo GetGetMethod();
        PropertyInfo PropertyInfo { get; }
        bool Indexer { get; }
    }

    partial interface IPropertyCaller<in T> : IMemberCaller<T>, IPropertyCaller
    {

    }
}