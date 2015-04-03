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