using System;
using System.Reflection;

namespace Horizon
{
    class MemberCaller<T> : IMemberCaller<T>
    {
        protected readonly MemberInfo _info;
        private readonly Lazy<Action<T, object>> _setter;
        private readonly Lazy<Func<T, object>> _getter;

        public readonly bool CanWrite;
        public readonly bool CanRead;

        public MemberInfo MemberInfo { get { return _info; } }
        public string Name { get; private set; }
        public Type MemberType { get; private set; }
        public bool IsProperty { get; private set; }
        public bool IsField { get { return !IsProperty; } }

        private MemberCaller(MemberInfo info)
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



        public object Get(T instance)
        {
            return _getter.Value(instance);
        }

        public void Set(T instance, object value)
        {
            _setter.Value(instance, value);
        }

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




        object IMemberCaller.Get(object instance)
        {
            return Get((T) instance);
        }

        void IMemberCaller.Set(object instance, object value)
        {
            Set((T)instance, value);
        }

        bool IMemberCaller.TryGet(object instance, out object result)
        {
            return TryGet((T) instance, out result);
        }

        bool IMemberCaller.TrySet(object instance, object value)
        {
            return TrySet((T)instance, value);
        }
    }
}