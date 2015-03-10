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

namespace Horizon
{
    struct TypeHash
    {
        private readonly Type[] _types;
        private readonly Lazy<int> _hashCode;


        public TypeHash(IEnumerable<Type> types)
        {
            var typeArray = _types = types.ToArray();
            _hashCode = new Lazy<int>(() => unchecked(typeArray.Aggregate(17, (current, element) => current*31 + element.GetHashCode())));
        }

        public IReadOnlyList<Type> Types
        {
            get { return _types; }
        }

        public bool Equals(TypeHash other)
        {
            return _types.SequenceEqual(other._types);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeHash && Equals((TypeHash) obj);
        }

        public static bool operator ==(TypeHash left, TypeHash right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TypeHash left, TypeHash right)
        {
            return !left.Equals(right);
        }
        
        public override int GetHashCode()
        {
            return _hashCode.Value;
        }
    }
}