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
using System.Reflection;

namespace Horizon
{
    partial class SimpleParameterInfo
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

        public string Name { get; private set; }
        public object DefaultValue { get; private set; }
        public Type ParameterType { get; private set; }
        public Type OriginalParameterType { get; private set; }
        public bool HasDefaultValue { get; private set; }
    }
}