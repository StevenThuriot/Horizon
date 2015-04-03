using System;
using System.ComponentModel;

namespace Horizon
{
    class SelectableArgument : Argument
    {
        private Type _actualType;

        public SelectableArgument(SimpleParameterInfo parameterInfo)
            : base(parameterInfo.Name, parameterInfo.DefaultValue, parameterInfo.ParameterType)
        {
            HasDefaultValue = parameterInfo.HasDefaultValue;
        }

        public bool HasDefaultValue { get; private set; }

        public bool Selected { get; private set; }

        public void SelectFor(Type type)
        {
            _actualType = type;
            Selected = true;
        }


        public dynamic GetValue()
        {
            var outType = Type;

            if (outType == null) return false;

            var actualType = _actualType;

            if (outType.IsGenericTypeDefinition)
            {
                Type[] genericParameters;
                if (actualType.IsGenericTypeOf(outType, out genericParameters))
                {
                    outType = outType.GetGenericTypeDefinition().MakeGenericType(genericParameters);
                }
            }

            var value = Value;
            if (outType.IsAssignableFrom(actualType)) return value;

            if (Reference.IsNull(value))
                return null;

            var converter = TypeDescriptor.GetConverter(actualType);
            if (converter.CanConvertTo(outType))
                return converter.ConvertTo(value, outType);

            converter = TypeDescriptor.GetConverter(outType);
            if (converter.CanConvertFrom(actualType))
                return converter.ConvertFrom(value);

            
            dynamic dynamicValue = value;
            dynamic result;

            if (TypeInfo.TryImplicitConvert(dynamicValue, Type, out result))
                return result;

            return value;
        }

        public void SelectFor(Argument parameter)
        {
            Value = parameter.Value;
            SelectFor(parameter.Type);
        }
    }
}