using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    class NullCaller : IInternalCaller
    {
        public static IInternalCaller Instance = new NullCaller();

        private NullCaller()
        {
        }


        public string Name { get { return "null"; } }
        public object Call(IEnumerable<dynamic> values)
        {
            return null;
        }

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes
        {
            get { return Enumerable.Empty<SimpleParameterInfo>().ToArray(); }
        }

        public bool IsStatic { get { return false; }}
    }
}