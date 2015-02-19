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

namespace Invocation
{
    struct TypeHash
    {
        private readonly Type[] _types;

        public TypeHash(IEnumerable<Type> types)
        {
            _types = types.ToArray();
        }

        public IReadOnlyList<Type> Types
        {
            get { return _types; }
        }

        public bool Equals(TypeHash other)
        {
            //if (_types == null)
            //    return other._types == null;

            //if (other._types == null)
            //    return false;

            return _types.SequenceEqual(other._types);
        }

        public override bool Equals(object obj)
        {
            //if (ReferenceEquals(null, obj)) return false;
            return obj is TypeHash && Equals((TypeHash) obj);
        }

        public static bool operator ==(TypeHash left, TypeHash right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeHash left, TypeHash right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                //if (_types == null)
                //    return 0;

                var elementComparer = EqualityComparer<Type>.Default;
                return _types.Aggregate(17, (current, element) => current*31 + elementComparer.GetHashCode(element));
            }
        }
    }
}