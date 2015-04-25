using System;
using System.Reflection;

namespace Horizon
{
    partial interface IMethodCaller : ICaller
    {
        Type ReturnType { get; }
        MethodInfo MethodInfo { get; }
    }
}