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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Invocation
{
    class MethodCaller
    {
        public readonly string Name;
        public readonly IReadOnlyList<ParameterInfo> ParameterTypes;
        private readonly Lazy<Delegate> _caller;
        private readonly MethodInfo _info;

        public MethodCaller(MethodInfo info)
        {
            _info = info;
            Name = info.Name;
            ParameterTypes = info.GetParameters();
            _caller = info.BuildLazy();
        }

        public bool IsAsync
        {
            get { return typeof (Task).IsAssignableFrom(_info.ReturnType); }
        }

        public bool IsStatic {
            get { return _info.IsStatic; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MethodCaller) obj);
        }
        protected bool Equals(MethodCaller other)
        {
            return string.Equals(Name, other.Name) && Equals(ParameterTypes, other.ParameterTypes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode()*397) ^ ParameterTypes.GetHashCode();
            }
        }


        public object Call(IEnumerable<object> values)
        {
            dynamic[] arguments = values.ToArray<dynamic>();
            return _caller.Value.FastInvoke(arguments);
        }
    }
}