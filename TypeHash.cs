using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon
{
    struct TypeHash
    {
        readonly Type[] _types;
        readonly Lazy<int> _hashCode;


        public TypeHash(IEnumerable<Type> types)
        {
            var typeArray = _types = types.ToArray();
            _hashCode = new Lazy<int>(() => unchecked(typeArray.Aggregate(17, (current, element) => current*31 + element.GetHashCode())));
        }

        public IReadOnlyList<Type> Types => _types;

        public bool Equals(TypeHash other) => _types.SequenceEqual(other._types);

        public override bool Equals(object obj) => obj is TypeHash && Equals((TypeHash)obj);

        public static bool operator ==(TypeHash left, TypeHash right) => left.Equals(right);

        public static bool operator !=(TypeHash left, TypeHash right) => !left.Equals(right);

        public override int GetHashCode() => _hashCode.Value;
    }
}