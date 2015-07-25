using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;
using System.Runtime.ExceptionServices;

namespace Horizon
{
	static partial class InvokeHelper
    {
        private static readonly Type _voidType = typeof(void);

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
            if (method.ReturnType == _voidType)
            {
                var returnLabel = Expression.Label(Expression.Label(typeof(object), "result"),
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

        
	    public static Delegate BuildCallSite(this MethodInfo method, IEnumerable<object> variables)
        {
            var argumentCounter = variables.Count();
            
            var callFlag = CSharpArgumentInfoFlags.None;

	        if (method.IsStatic)
	            callFlag = callFlag | CSharpArgumentInfoFlags.IsStaticType;

            
	        var argumentInfo = new List<CSharpArgumentInfo>
                               {
                                   CSharpArgumentInfo.Create(callFlag, null)
                               };

            for (var i = 0; i < argumentCounter; i++)
            {
                //TODO: Attempt to resolve flags:
                //IsRef             CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.IsRef, null)
                //IsOut             CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.IsOut, null)
                //NamedArgument     CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "the name goes here")
                var argument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
                argumentInfo.Add(argument);
            }
            var hasReturnType = method.ReturnType != _voidType;

	        var binderFlags = hasReturnType ? CSharpBinderFlags.None : CSharpBinderFlags.ResultDiscarded;
	        var callSiteBinder = Binder.InvokeMember(binderFlags, method.Name, null, method.DeclaringType, argumentInfo);

            var signature = ResolveSignature(method, argumentCounter);

	        var callsite = Expression.Constant(CallSite.Create(signature, callSiteBinder));
            var parameters = new List<Expression>
                             {
                                 callsite
                             };

            if (method.IsStatic)
                parameters.Add(Expression.Constant(method.DeclaringType));
            
            for (var i = 0; i < argumentCounter; i++)
                parameters.Add(Expression.Parameter(typeof(object)));

            var target = Expression.Field(callsite, "Target");
            var call = Expression.Invoke(target, parameters);
            var lambda = Expression.Lambda(call, parameters.OfType<ParameterExpression>());
            var d = lambda.Compile();

	        return d;
        }


		public static Lazy<Delegate> BuildLazy(this ConstructorInfo ctor)
		{
			return new Lazy<Delegate>(() => Build(ctor));
		}

		public static Lazy<T> BuildLazy<T>(this ConstructorInfo ctor)
		{
			if (ctor == null) throw new ArgumentNullException("ctor");
			return new Lazy<T>(() => Build<T>(ctor));
		}

		public static Delegate Build(this ConstructorInfo ctor)
		{
			IEnumerable<ParameterExpression> allParameters;
			var ctorExpression = BuildExpression(ctor, out allParameters);

			var lambda = Expression.Lambda(ctorExpression, "ctor_invoker", allParameters);
			var func = lambda.Compile();

			return func;
		}

		public static T Build<T>(this ConstructorInfo ctor)
		{
			IEnumerable<ParameterExpression> allParameters;
			var ctorExpression = BuildExpression(ctor, out allParameters);

			var lambda = Expression.Lambda<T>(ctorExpression, "ctor_invoker", allParameters);
			var func = lambda.Compile();

			return func;
		}

		private static Expression BuildExpression(ConstructorInfo ctor, out IEnumerable<ParameterExpression> allParameters)
		{
			var parameters = ctor.GetParameters()
								 .Select(x => Expression.Parameter(x.ParameterType, x.Name))
								 .ToList();

			var caller = Expression.New(ctor, parameters);

			allParameters = parameters;
			return caller;
		}



        public static object FastInvoke(this Delegate @delegate, params dynamic[] args)
        {
            if (@delegate.Method.ReturnType != _voidType)
            {
                switch (args.Length)
                {
                    case 0:
                        return ((dynamic)@delegate)();

                    case 1:
                        return ((dynamic)@delegate)(args[0]);

                    case 2:
                        return ((dynamic)@delegate)(args[0], args[1]);

                    case 3:
                        return ((dynamic)@delegate)(args[0], args[1], args[2]);

                    case 4:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3]);

                    case 5:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4]);

                    case 6:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5]);

                    case 7:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);

                    case 8:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);

                    case 9:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);

                    case 10:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);

                    case 11:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);

                    case 12:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);

                    case 13:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);

                    case 14:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);

                    case 15:
                        return ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]);


                    default:
                        try
                        {
                            return @delegate.DynamicInvoke(args.Cast<object>().ToArray());
                        }
                        catch (TargetInvocationException ex)
                        {
                            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                        }
                        break;
                }
            }
            else
            {
                switch (args.Length)
                {
                    case 0:
                        ((dynamic)@delegate)();
                        break;

                    case 1:
                        ((dynamic)@delegate)(args[0]);
                        break;

                    case 2:
                        ((dynamic)@delegate)(args[0], args[1]);
                        break;

                    case 3:
                        ((dynamic)@delegate)(args[0], args[1], args[2]);
                        break;

                    case 4:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3]);
                        break;

                    case 5:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4]);
                        break;

                    case 6:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5]);
                        break;

                    case 7:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                        break;

                    case 8:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                        break;

                    case 9:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                        break;

                    case 10:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                        break;

                    case 11:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
                        break;

                    case 12:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
                        break;

                    case 13:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
                        break;

                    case 14:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
                        break;

                    case 15:
                        ((dynamic)@delegate)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]);
                        break;


                    default:
                        try
                        {
                            @delegate.DynamicInvoke(args.Cast<object>().ToArray());
                        }
                        catch (TargetInvocationException ex)
                        {
                            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                        }
                        break;
                }

                return null;
            }

            throw new NotSupportedException("Invocation failure");
        }



        private static Type ResolveSignature(MethodInfo method, int argumentCounter)
        {
            var hasReturnType = method.ReturnType != _voidType;

            if (method.IsStatic)
            {
                switch (argumentCounter)
                {
                    case 0:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object>)
                            : typeof(Action<CallSite, Type>);

                    case 1:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object>)
                            : typeof(Action<CallSite, Type, object>);

                    case 2:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object>);

                    case 3:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object>);

                    case 4:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object>);

                    case 5:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object>);

                    case 6:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object>);

                    case 7:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object>);

                    case 8:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object>);

                    case 9:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object, object>);

                    case 10:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object, object, object>);

                    case 11:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object>);

                    case 12:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object>);

                    case 13:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object, object>);

                    case 14:
                        return hasReturnType
                            ? typeof(Func<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                            : typeof(Action<CallSite, Type, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);



                    default:
                        throw new NotSupportedException("Too many arguments.");
                }
            }


            switch (argumentCounter)
            {
                case 0:
                    return hasReturnType
                        ? typeof(Func<CallSite, object>)
                        : typeof(Action<CallSite>);

                case 1:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object>)
                        : typeof(Action<CallSite, object>);

                case 2:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object>)
                        : typeof(Action<CallSite, object, object>);

                case 3:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object>);

                case 4:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object>);

                case 5:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object>);

                case 6:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object>);

                case 7:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object>);

                case 8:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object>);

                case 9:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object>);

                case 10:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object, object>);

                case 11:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object, object, object>);

                case 12:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>);

                case 13:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>);

                case 14:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);

                case 15:
                    return hasReturnType
                        ? typeof(Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)
                        : typeof(Action<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);



                default:
                    throw new NotSupportedException("Too many arguments.");
            }
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

            var parameter = Expression.Parameter(typeof(T), "instance");
            var memberExpression = Expression.Field(parameter, info);
            var boxExpression = Expression.Convert(memberExpression, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

            return lambda.Compile();
        }

        public static Action<T, object> CreateSetter(FieldInfo info)
        {
            if (info.IsStatic)
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(typeof(T), "instance");
            var memberExpression = Expression.Field(parameter, info);
            return BuildSetter(parameter, memberExpression, info.FieldType);
        }

        public static Func<T, object> CreateGetter(PropertyInfo info)
        {
            if (IsStatic(info))
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(typeof(T), "instance");
            var memberExpression = Expression.Property(parameter, info);
            var boxExpression = Expression.Convert(memberExpression, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(boxExpression, parameter);

            return lambda.Compile();
        }

        public static Action<T, object> CreateSetter(PropertyInfo info)
        {
            if (IsStatic(info))
                throw new NotSupportedException("Static fields are not supported.");

            var parameter = Expression.Parameter(typeof(T), "instance");
            var memberExpression = Expression.Property(parameter, info);
            return BuildSetter(parameter, memberExpression, info.PropertyType);
        }


        private static Action<T, object> BuildSetter(ParameterExpression parameter, Expression memberExpression,
                                                     Type valueType)
        {
            var value = Expression.Parameter(typeof(object), "value");
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
