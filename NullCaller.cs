using System;
using System.Collections.Generic;
using System.Reflection;

namespace Horizon
{
    class NullCaller : IMethodCaller
    {
        public static readonly IMethodCaller Instance = new NullCaller();

        NullCaller()
        {
            ParameterTypes = new SimpleParameterInfo[0];
        }


        public string Name => "null";

        public object Call(IEnumerable<dynamic> values) => null;

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes { get; }

        public bool IsStatic => false;

        public Type ReturnType => typeof(object);

        public MethodInfo MethodInfo => null;
    }
}