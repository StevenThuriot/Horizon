using System;
using System.ComponentModel;

namespace Horizon
{
	class Argument
	{
		public readonly string Name;
		public readonly Type Type;

		public Argument(string name, object value, Type type = null)
		{
			Name = name;
			Value = value;

			if (type != null)
			{
				Type = type;
			}
			else if (value != null)
			{
				Type = value.GetType();
			}
		}

		public object Value { get; set; }

        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        public bool IsAssignableTo(Type parameterType)
        {
            var actualType = Type;
            if (actualType == null)
                return false;

		    if (parameterType.IsGenericTypeDefinition && actualType.IsGenericType)
		        return actualType.IsGenericTypeOf(parameterType);


		    if (parameterType.IsAssignableFrom(actualType))
                return true;
            
			var converter = TypeDescriptor.GetConverter(actualType);
			if (converter.CanConvertTo(parameterType))
				return true;

			converter = TypeDescriptor.GetConverter(parameterType);
			if (converter.CanConvertFrom(actualType))
				return true;

		    var value = Value;
		    if (Reference.IsNull(value))
                return false;
            

		    //Resolve T through DLR
		    dynamic dynamicValue = value;
		    return Info.CanImplicitConvert(dynamicValue, parameterType);
		}
	}
}