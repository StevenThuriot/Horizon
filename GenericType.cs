using System;
using System.Linq;

namespace Horizon
{
    static class GenericType
    {
        public static bool IsGenericTypeOf(this Type type, Type genericDefinition)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition())
                return true;

            if (genericDefinition.IsInterface && type.GetInterfaces().Any(i => IsGenericTypeOf(i, genericDefinition)))
                return true;

            if (type.BaseType != null && IsGenericTypeOf(type.BaseType, genericDefinition))
                return true;

            return false;
        }

        public static bool IsGenericTypeOf(this Type type, Type genericDefinition, out Type[] genericParameters)
        {
            genericParameters = Type.EmptyTypes;

            var isMatch = type.IsGenericType &&
                          type.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();

            if (!isMatch && type.BaseType != null)
                isMatch = type.BaseType.IsGenericTypeOf(genericDefinition, out genericParameters);

            if (!isMatch && genericDefinition.IsInterface)
            {
                var interfaces = type.GetInterfaces();
                if (interfaces.Length != 0)
                {
                    foreach (var i in interfaces)
                    {
                        if (i.IsGenericTypeOf(genericDefinition, out genericParameters))
                        {
                            isMatch = true;
                            break;
                        }
                    }
                }
            }

            if (isMatch && !genericParameters.Any())
                genericParameters = type.GetGenericArguments();


            return isMatch;
        }
    }
}