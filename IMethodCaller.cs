using System;

namespace Horizon
{
    partial interface IMethodCaller : ICaller
    {
        Type ReturnType { get; } 
    }
}