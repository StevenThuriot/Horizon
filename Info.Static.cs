using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    partial class Info
    {
        public static class Static
        {
            static IDictionary<Type, IInfoContainer> _cache = new Dictionary<Type, IInfoContainer>();
            static IInfoContainer ResolveInfoContainer(Type type)
            {
                IInfoContainer container;

                if (!_cache.TryGetValue(type, out container))
                {
                    var containerType = typeof(InfoContainer<>).MakeGenericType(type);
                    _cache[type] = container = Info.Create(containerType);
                }

                return container;
            }
            
            public static IEnumerable<IMethodCaller> Methods(Type type) => ResolveInfoContainer(type).Methods.SelectMany(x => x);

            public static IEnumerable<IEventCaller> Events(Type type) => ResolveInfoContainer(type).Events.Values;

            public static IEnumerable<IPropertyCaller> Properties(Type type) => ResolveInfoContainer(type).Properties.Values;

            public static IEnumerable<IMemberCaller> Fields(Type type) => ResolveInfoContainer(type).Fields.Values;

            public static IEnumerable<IMemberCaller> Members(Type type)
            {
                foreach (var property in Properties(type))
                    yield return property;

                foreach (var property in Fields(type))
                    yield return property;
            }


            //TODO: Call, get, set
        }
    }
}
