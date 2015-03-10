#region License

//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

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