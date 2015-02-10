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
using System.Linq.Expressions;
using System.Reflection;

namespace Invocation
{
	public static partial class InvokeHelper
    {
        public static Lazy<Delegate> BuildLazy(this MethodInfo method)
        {
            return new Lazy<Delegate>(() => Build(method));
        }

        public static Lazy<T> BuildLazy<T>(this MethodInfo method)
        {
            return new Lazy<T>(() => Build<T>(method));
        }

        public static Delegate Build(this MethodInfo method)
        {
            IEnumerable<ParameterExpression> allParameters;
            var wrapper = BuildExpression(method, out allParameters);

            var lambda = Expression.Lambda(wrapper, "invoker", allParameters);
            var func = lambda.Compile();

            return func;
        }

        public static T Build<T>(this MethodInfo method)
        {
            IEnumerable<ParameterExpression> allParameters;
            var wrapper = BuildExpression(method, out allParameters);

            var lambda = Expression.Lambda<T>(wrapper, "invoker", allParameters);
            var func = lambda.Compile();

            return func;
        }

        private static Expression BuildExpression(MethodInfo method, out IEnumerable<ParameterExpression> allParameters)
        {
            var parameters = method.GetParameters()
                                   .Select(x => Expression.Parameter(x.ParameterType, x.Name))
                                   .ToList();

            MethodCallExpression call;

            if (method.IsStatic)
            {
                call = Expression.Call(method, parameters);
            }
            else
            {
                var instance = Expression.Parameter(method.DeclaringType, "instance");
                call = Expression.Call(instance, method, parameters);
            }


            Expression wrapper;
            if (method.ReturnType == typeof (void))
            {
                var returnLabel = Expression.Label(Expression.Label(typeof (object), "result"),
                                                   Expression.Constant(null));

                wrapper = Expression.Block
                    (
                     call,
                     Expression.Return(returnLabel.Target, returnLabel.DefaultValue, returnLabel.Target.Type),
                     returnLabel
                    );
            }
            else
            {
                wrapper = call;
            }

            var parameterExpressions = parameters.ToList();

            var instanceExpression = call.Object as ParameterExpression;
            if (instanceExpression != null) parameterExpressions.Insert(0, instanceExpression);

            allParameters = parameterExpressions;
            return wrapper;
        }
    }

    static class InvokeHelper<T>
    {
        public static Lazy<Func<T, object>> CreateGetterLazy(PropertyInfo info)
        {
            return new Lazy<Func<T, object>>(() => CreateGetter(info));
        }

        public static Lazy<Func<T, object>> CreateGetterLazy(FieldInfo info)
        {
            return new Lazy<Func<T, object>>(() => CreateGetter(info));
        }

        public static Lazy<Action<T, object>> CreateSetterLazy(PropertyInfo info)
        {
            return new Lazy<Action<T, object>>(() => CreateSetter(info));
        }

        public static Lazy<Action<T, object>> CreateSetterLazy(FieldInfo info)
        {
            return new Lazy<Action<T, object>>(() => CreateSetter(info));
        }


        public static Func<T, object> CreateGetter(FieldInfo info)
        {
            if (info.IsStatic)
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var memberExpression = Expression.Field(parameter, info);
            var boxExpression = Expression.Convert(memberExpression, typeof (object));
            var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

            return lambda.Compile();
        }

        public static Action<T, object> CreateSetter(FieldInfo info)
        {
            if (info.IsStatic)
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var memberExpression = Expression.Field(parameter, info);
            return BuildSetter(parameter, memberExpression, info.FieldType);
        }

        public static Func<T, object> CreateGetter(PropertyInfo info)
        {
            if (IsStatic(info))
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var memberExpression = Expression.Property(parameter, info);
            var boxExpression = Expression.Convert(memberExpression, typeof (object));
            var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

            return lambda.Compile();
        }

        public static Action<T, object> CreateSetter(PropertyInfo info)
        {
            if (IsStatic(info))
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(Constants.Typed<T>.OwnerType, "instance");
            var memberExpression = Expression.Property(parameter, info);
            return BuildSetter(parameter, memberExpression, info.PropertyType);
        }


        private static Action<T, object> BuildSetter(ParameterExpression parameter, Expression memberExpression,
                                                     Type valueType)
        {
            var value = Expression.Parameter(Constants.ObjectType, "value");
            var unboxedValue = Expression.Convert(value, valueType);
            var assign = Expression.Assign(memberExpression, unboxedValue);

            var lambda = Expression.Lambda<Action<T, object>>(assign, parameter, value);

            return lambda.Compile();
        }

        private static bool IsStatic(PropertyInfo propertyInfo)
        {
            return ((propertyInfo.CanRead && propertyInfo.GetMethod.IsStatic) ||
                    (propertyInfo.CanWrite && propertyInfo.SetMethod.IsStatic));
        }
    }
}
