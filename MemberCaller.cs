using System;
using System.Reflection;

namespace Horizon
{
    class MemberCaller<T> : IMemberCaller<T>
    {
        protected readonly MemberInfo _info;
        readonly Lazy<Action<T, object>> _setter;
        readonly Lazy<Func<T, object>> _getter;

        public bool CanWrite { get; }
        public bool CanRead { get; }

        public MemberInfo MemberInfo => _info;
        public string Name { get; }
        public Type MemberType { get; }
        public bool IsProperty { get; }
        public bool IsField => !IsProperty;


        MemberCaller(MemberInfo info)
        {
            _info = info;
            Name = info.Name;
        }

        public MemberCaller(PropertyInfo info)
            : this((MemberInfo)info)
        {
            MemberType = info.PropertyType;
            IsProperty = true;

            if (info.CanWrite)
            {
                CanWrite = true;
                _setter = InvokeHelper<T>.CreateSetterLazy(info);
            }
            else
            {
                CanWrite = false;
            }

            if (info.CanRead)
            {
                CanRead = true;
                _getter = InvokeHelper<T>.CreateGetterLazy(info);
            }
            else
            {
                CanRead = false;
            }
        }

        public MemberCaller(FieldInfo info)
            : this((MemberInfo)info)
        {
            MemberType = info.FieldType;
            IsProperty = false;

            if (!info.IsInitOnly)
            {
                CanWrite = true;
                _setter = InvokeHelper<T>.CreateSetterLazy(info);
            }
            else
            {
                CanWrite = false;
            }

            CanRead = true;
            _getter = InvokeHelper<T>.CreateGetterLazy(info);
        }



        public object Get(T instance) => _getter.Value(instance);

        public void Set(T instance, object value) => _setter.Value(instance, value);

        public bool TryGet(T instance, out object result)
        {
            if (CanRead)
            {
                result = _getter.Value(instance);
                return true;
            }

            result = null;
            return false;
        }

        public bool TrySet(T instance, object value)
        {
            if (CanWrite)
            {
                _setter.Value(instance, value);
                return true;
            }

            return false;
        }




        object IMemberCaller.Get(object instance) => Get((T)instance);

        void IMemberCaller.Set(object instance, object value) => Set((T)instance, value);

        bool IMemberCaller.TryGet(object instance, out object result) => TryGet((T)instance, out result);

        bool IMemberCaller.TrySet(object instance, object value) => TrySet((T)instance, value);
    }
}