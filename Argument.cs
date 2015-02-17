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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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

		public bool IsAssignableTo(Type type)
        {
            var inType = Type;
            if (inType == null) return false;

		    if (type.IsGenericTypeDefinition && inType.IsGenericType)
		    {
		        Debugger.Break();
		        var arguments = inType.GetGenericArguments();
		        type = type.MakeGenericType(arguments);
		    }


		    if (type.IsAssignableFrom(inType)) return true;
            
			var converter = TypeDescriptor.GetConverter(inType);
			if (converter.CanConvertTo(type))
				return true;

			converter = TypeDescriptor.GetConverter(type);
			if (converter.CanConvertFrom(inType))
				return true;

		    var value = Value;
		    if (ReferenceEquals(null, value)) return false;
            

		    //Resolve T through DLR
		    dynamic dynamicValue = value;
		    return TypeInfo.CanImplicitConvert(dynamicValue, type);
		}
	}
}