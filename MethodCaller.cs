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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Invocation
{
    [DebuggerDisplay("{GetType().Name} - {Info.ToString()}")]
    class MethodCaller
    {
        public static MethodCaller Create(MethodInfo info)
        {
            if (info.IsGenericMethodDefinition)
                return new GenericMethodCaller(info);

            return new MethodCaller(info);
        }


        public readonly string Name;
        public readonly IReadOnlyList<SimpleParameterInfo> ParameterTypes;
        private readonly Lazy<Delegate> _caller;
        private readonly Lazy<bool> _isAsync;

        protected readonly MethodInfo Info;

        internal MethodCaller(MethodInfo info)
            :this(info, info.GetParameters().Select(x =>new SimpleParameterInfo(x)))
        {
            _caller = info.BuildLazy();
        }

        protected MethodCaller(MethodInfo info, IEnumerable<SimpleParameterInfo> parameterTypes)
        {
            Info = info;
            Name = info.Name;
            ParameterTypes = parameterTypes.ToArray();
            _isAsync = new Lazy<bool>(() => typeof(Task).IsAssignableFrom(info.ReturnType));
        }

        public bool IsAsync
        {
            get { return _isAsync.Value; }
        }

        public bool IsStatic
        {
            get { return Info.IsStatic; }
        }

        public Type ReturnType
        {
            get { return Info.ReturnType; }
        }


        public override bool Equals(object obj)
        {
            if (Reference.IsNull(obj)) return false;
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


        public virtual object Call(IEnumerable<dynamic> values)
        {
            var arguments = values.ToArray();
            return _caller.Value.FastInvoke(arguments);
        }
    }
}