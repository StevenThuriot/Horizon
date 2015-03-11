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

namespace Horizon
{
    class ConstructorCaller : IInternalCaller
    {
        protected readonly ConstructorInfo Info;

        public readonly string Name;
        private readonly Lazy<Delegate> _caller;

        internal ConstructorCaller(ConstructorInfo info)
            : this(info, info.GetParameters().Select(x => new SimpleParameterInfo(x)))
        {
            _caller = info.BuildLazy();
        }

        protected ConstructorCaller(ConstructorInfo info, IEnumerable<SimpleParameterInfo> parameterTypes)
        {
            Info = info;
            Name = info.Name;
            ParameterTypes = parameterTypes.ToArray();
        }

        public IReadOnlyList<SimpleParameterInfo> ParameterTypes { get; private set; }

        public bool IsStatic
        {
            get { return Info.IsStatic; }
        }

        public virtual object Call(IEnumerable<dynamic> values)
        {
            var arguments = values.ToArray();
            return _caller.Value.FastInvoke(arguments);
        }


        public override bool Equals(object obj)
        {
            if (Reference.IsNull(obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConstructorCaller) obj);
        }

        protected bool Equals(ConstructorCaller other)
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
    }
}