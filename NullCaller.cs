﻿using System.Collections.Generic;

namespace Horizon
{
    class NullCaller : IInternalCaller
    {
        public static IInternalCaller Instance = new NullCaller();

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
    }
}