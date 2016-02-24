using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Horizon
{
    class InfoContainer<T>
    {
        public readonly ILookup<string, MethodCaller> Methods;

        public readonly IReadOnlyList<ConstructorCaller> Constructors;

        public readonly IReadOnlyDictionary<string, EventCaller> Events;

        public readonly IReadOnlyDictionary<string, PropertyCaller<T>> Properties;
        public readonly IReadOnlyList<PropertyCaller<T>> Indexers;

        public readonly IReadOnlyDictionary<string, MemberCaller<T>> Fields;

        public InfoContainer()
        {
            var ctors = new List<ConstructorCaller>();
            Constructors = ctors;

            var methods = new List<MethodCaller>();

            var events = new List<EventCaller>();

            var props = new Dictionary<string, PropertyCaller<T>>();
            Properties = props;

            var fields = new Dictionary<string, MemberCaller<T>>();
            Fields = fields;

            var indexers = new List<PropertyCaller<T>>();
            Indexers = indexers;

            foreach (var member in typeof(T).GetMembers(BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static //default getMember flags
                                                                                | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)) //Our additional flags
            {
                var key = member.Name;

                if ((MemberTypes.Property & member.MemberType) == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)member;
                    var propertyCaller = new PropertyCaller<T>(propertyInfo);
                    props[key] = propertyCaller;

                    if (propertyCaller.IsIndexer)
                        indexers.Add(propertyCaller);
                }
                else if ((MemberTypes.Field & member.MemberType) == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo)member;
                    fields[key] = new MemberCaller<T>(fieldInfo);
                }
                else if ((MemberTypes.Method & member.MemberType) == MemberTypes.Method)
                {
                    var methodInfo = (MethodInfo)member;
                    if (!methodInfo.IsSpecialName)
                    {
                        //Skip Properties, events, ... 
                        var caller = MethodCaller.Create(methodInfo);
                        methods.Add(caller);
                    }
                }
                else if ((MemberTypes.Constructor & member.MemberType) == MemberTypes.Constructor)
                {
                    var constructorInfo = (ConstructorInfo)member;
                    var caller = new ConstructorCaller(constructorInfo);
                    ctors.Add(caller);
                }
                else if ((MemberTypes.Event & member.MemberType) == MemberTypes.Event)
                {
                    var eventInfo = (EventInfo)member;
                    var caller = new EventCaller(eventInfo);

                    events.Add(caller);
                }
            }

            Methods = methods.OrderBy(x => x is GenericMethodCaller)//this will make sure non-generic caller are prefered.
                              .ToLookup(x => x.Name, x => x);

            Events = events.ToDictionary(x => x.Name);
        }
    }
}
