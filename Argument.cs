﻿#region License
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

namespace Invocation
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

		public bool HasName
		{
			get { return !string.IsNullOrWhiteSpace(Name); }
		}

		public bool IsAssignableTo(Type parameterType)
        {
            var actualType = Type;
            if (actualType == null) return false;

		    if (parameterType.IsGenericTypeDefinition && actualType.IsGenericType)
		        return actualType.IsGenericTypeOf(parameterType);


		    if (parameterType.IsAssignableFrom(actualType)) return true;
            
			var converter = TypeDescriptor.GetConverter(actualType);
			if (converter.CanConvertTo(parameterType))
				return true;

			converter = TypeDescriptor.GetConverter(parameterType);
			if (converter.CanConvertFrom(actualType))
				return true;

		    var value = Value;
		    if (Reference.IsNull(value)) return false;
            

		    //Resolve T through DLR
		    dynamic dynamicValue = value;
		    return TypeInfo.CanImplicitConvert(dynamicValue, parameterType);
		}
	}
}