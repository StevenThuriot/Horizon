using System;
using System.Reflection;

namespace Horizon
{
	struct SimpleParameterInfo
    {
        public SimpleParameterInfo(string name, object defaultValue, Type parameterType, Type originalParameterType,
                                   bool hasDefaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            ParameterType = parameterType;
            OriginalParameterType = originalParameterType;
            HasDefaultValue = hasDefaultValue;
        }

        public SimpleParameterInfo(string name, object defaultValue, Type parameterType, bool hasDefaultValue)
            : this(name, defaultValue, parameterType, parameterType, hasDefaultValue)
        {
        }

        public SimpleParameterInfo(ParameterInfo parameterInfo)
            : this(
                parameterInfo.Name, parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : null,
                parameterInfo.ParameterType, parameterInfo.HasDefaultValue)
        {
        }

		public readonly string Name;
		public readonly object DefaultValue;
		public readonly Type ParameterType;
		public readonly Type OriginalParameterType;
		public readonly bool HasDefaultValue;
    }
}