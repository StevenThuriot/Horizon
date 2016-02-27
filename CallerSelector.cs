using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Horizon
{
	static class CallerSelector
	{
        static bool CompareParameters(IReadOnlyList<Argument> parameters, ICaller key, out IEnumerable<SelectableArgument> arguments)
        {
            var selectableArguments = key.ParameterTypes.Select(x => new SelectableArgument(x))
                                         .ToList();

            arguments = selectableArguments;

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                var methodParam = parameter.HasName
                                      ? selectableArguments.FirstOrDefault(x => x.Name == parameter.Name)
                                      : selectableArguments.ElementAtOrDefault(i);
                //Unnamed params come first and in order, select by index.

                //Check if a param has been found.
                if (methodParam == null)
                    return false;

                if (parameter.Type == null)
                {
                    //If null, null has been passed so the type has to be a value type.
                    if (!methodParam.Type.IsValueType)
                        return false;
                }
                else
                {
                    //If not null, parameter has to be assignable to 
                    if (!parameter.IsAssignableTo(methodParam.Type))
                        return false;
                }

                //Override value with passed value
                methodParam.SelectFor(parameter);
            }

            return selectableArguments.All(x => x.Selected || x.HasDefaultValue);
        }


        static readonly Stack<string> EmptyStack = new Stack<string>();

        public static Tuple<ICaller, IReadOnlyList<dynamic>> SelectMethod(IEnumerable<ICaller> callers, IEnumerable<object> args)
		{
			if (callers == null || !callers.Any())
				return null;

			var arguments = args.ToArray();
			return SelectMethod(callers, arguments, EmptyStack);
		}

        public static Tuple<ICaller, IReadOnlyList<dynamic>> SelectMethod(CallInfo callInfo, IEnumerable<ICaller> callers, IEnumerable<object> args)
		{
			if (callers == null || !callers.Any())
				return null;

			var arguments = args.ToArray();

			//Build argument list from binder
			var names = new Stack<string>(callInfo.ArgumentNames);

			return SelectMethod(callers, arguments, names);
		}

        static Tuple<ICaller, IReadOnlyList<dynamic>> SelectMethod(IEnumerable<ICaller> callers, IReadOnlyList<object> arguments, Stack<string> names)
		{
			var list = new List<Argument>();
			//Named parameters can always be mapped directly on the last parameters.
			for (var i = arguments.Count - 1; i != -1; i--)
			{
				var argument = arguments[i];
				string name = null;

				if (names.Count != 0)
					name = names.Pop();

				var arg = new Argument(name, argument);
				list.Insert(0, arg);
			}

			IEnumerable<SelectableArgument> actualArguments = null;

			//Compare argument list with relevant keys
			var selectedCaller = (from caller in callers
			                      where CompareParameters(list, caller, out actualArguments)
			                      select caller).FirstOrDefault();

			if (selectedCaller == null)
				return null;

            var argumentsResult = (IReadOnlyList<dynamic>)actualArguments.Select(x => x.GetValue()).ToArray();
            return Tuple.Create(selectedCaller, argumentsResult);
		}


		public static object Call(InvokeMemberBinder binder, IEnumerable<object> args, IEnumerable<MethodCaller> callers)
		{
			var method = SelectMethod(binder.CallInfo, callers, args);

			if (method == null)
                throw new ArgumentException("Invalid method name: " + binder.Name);

			var caller = method.Item1;
			var arguments = method.Item2;

			return caller.Call(arguments);
		}

		public static object Call(object instance, InvokeMemberBinder binder, IEnumerable<object> args, IEnumerable<MethodCaller> callers)
		{
			var method = SelectMethod(binder.CallInfo, callers, args);

			if (method == null)
                throw new ArgumentException("Invalid method name: " + binder.Name);

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			return caller.Call(arguments);
		}

		public static bool TryCall(InvokeMemberBinder binder, IEnumerable<object> args, IEnumerable<MethodCaller> callers, out object result)
		{
			var method = SelectMethod(binder.CallInfo, callers, args);

			if (method == null)
			{
				result = null;
				return false;
			}

			var caller = method.Item1;
			var arguments = method.Item2;

			result = caller.Call(arguments);
			return true;
		}

		public static bool TryCall(object instance, InvokeMemberBinder binder, IEnumerable<object> args, IEnumerable<MethodCaller> callers, out object result)
		{
			var method = SelectMethod(binder.CallInfo, callers, args);

			if (method == null)
			{
				result = null;
				return false;
			}

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			result = caller.Call(arguments);
			return true;
		}

		public static object Call(IEnumerable<object> args, IEnumerable<MethodCaller> callers)
		{
			var method = SelectMethod(callers, args);

			if (method == null)
                throw new ArgumentException("Invalid methodcall.");

			var caller = method.Item1;
			var arguments = method.Item2;

			return caller.Call(arguments);
		}

		public static object Call(object instance, IEnumerable<object> args, IEnumerable<MethodCaller> callers)
		{
			var method = SelectMethod(callers, args);

			if (method == null)
                throw new ArgumentException("Invalid methodcall.");

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			return caller.Call(arguments);
		}

		public static bool TryCall(IEnumerable<object> args, IEnumerable<MethodCaller> callers, out object result)
		{
			var method = SelectMethod(callers, args);

			if (method == null)
			{
				result = null;
				return false;
			}

			var caller = method.Item1;
			var arguments = method.Item2;

			result = caller.Call(arguments);
			return true;
		}

		public static bool TryCall(object instance, IEnumerable<object> args, IEnumerable<MethodCaller> callers, out object result)
		{
			var method = SelectMethod(callers, args);

			if (method == null)
			{
				result = null;
				return false;
			}

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			result = caller.Call(arguments);
			return true;
		}

		public static object GetIndexer(object instance, IEnumerable<IMethodCaller> callers, object[] indexes)
		{
			var method = SelectMethod(callers, indexes);

			if (method == null)
                throw new ArgumentException("Invalid Indexer");

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			return caller.Call(arguments);
		}

		public static void SetIndexer(object instance, IEnumerable<IMethodCaller> callers, object[] indexes, object value)
		{
			var input = indexes.ToList();
			input.Add(value);

			var method = SelectMethod(callers, input);

			if (method == null)
                throw new ArgumentException("Invalid Indexer");

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			caller.Call(arguments);
		}

		public static bool TryGetIndexer(object instance, IEnumerable<IMethodCaller> callers, object[] indexes, out object result)
		{
			var method = SelectMethod(callers, indexes);

			if (method == null)
			{
				result = null;
				return false;
			}

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			result = caller.Call(arguments);
			return true;
		}

		public static bool TrySetIndexer(object instance, IEnumerable<IMethodCaller> callers, object[] indexes, object value)
		{
			var input = indexes.ToList();
			input.Add(value);

			var method = SelectMethod(callers, input);

			if (method == null)
				return false;

			var caller = method.Item1;
			var arguments = method.Item2.ToList();

			if (!caller.IsStatic)
				arguments.Insert(0, instance);

			caller.Call(arguments);
			return true;
		}


		public static object Create(IEnumerable<ConstructorCaller> ctors, IEnumerable<object> arguments)
		{
			var ctor = SelectMethod(ctors, arguments);

			if (ctor == null)
                throw new ArgumentException("Invalid Ctor");

			var caller = ctor.Item1;
			var args = ctor.Item2.ToList();

			return caller.Call(args);
		}

		public static bool TryCreate(IEnumerable<ConstructorCaller> ctors, IEnumerable<object> arguments, out object instance)
		{
			var ctor = SelectMethod(ctors, arguments);

			if (ctor == null)
			{
				instance = null;
				return false;
			}

			var caller = ctor.Item1;
			var args = ctor.Item2.ToList();

			instance = caller.Call(args);
			return true;
		}
	}
}