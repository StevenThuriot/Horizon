using System;
using System.Collections.Generic;
using System.Reflection;

namespace Horizon
{
    class NullCaller : IMethodCaller
    {
        public static IMethodCaller Instance = new NullCaller();

        private NullCaller()
        {
            ParameterTypes = new SimpleParameterInfo[0];
        }


        public string Name { get { return "null"; } }

        public object Call(IEnumerable<dynamic> values)
        {
            return null;
        }

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes { get; private set; }

        public bool IsStatic { get { return false; }}

        public Type ReturnType
        {
            get { return Constants.ObjectType; }
        }

        public MethodInfo MethodInfo
        {
            get { return null; }
        }
    }
}