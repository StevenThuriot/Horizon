using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    class NullCaller : IInternalCaller
    {
        public static IInternalCaller Instance = new NullCaller();
        private SimpleParameterInfo[] _parameterTypes;

        private NullCaller()
        {
            _parameterTypes = new SimpleParameterInfo[0];
        }


        public string Name { get { return "null"; } }
        public object Call(IEnumerable<dynamic> values)
        {
            return null;
        }

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes
        {
            get { return _parameterTypes; }
        }

        public bool IsStatic { get { return false; }}
    }
}