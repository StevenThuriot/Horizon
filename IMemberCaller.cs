using System;
using System.Reflection;

namespace Horizon
{
    partial interface IMemberCaller<in T> : IMemberCaller
    {
        object Get(T instance);
        void Set(T instance, object value);
        bool TryGet(T instance, out object result);
        bool TrySet(T instance, object value);
    }
    partial interface IMemberCaller
    {
        MemberInfo MemberInfo { get; }
        string Name { get; }
        Type MemberType { get; }
        bool IsProperty { get; }
        bool IsField { get; }
        object Get(object instance);
        void Set(object instance, object value);
        bool TryGet(object instance, out object result);
        bool TrySet(object instance, object value);
    }
}