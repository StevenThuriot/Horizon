---
title: "Getting Started"
bg: darkgreen
color: black
fa-icon: code
---

## Getting Started with Horizon

### The basics

Everything you need is in the `Horizon` namespace, so start by including it

```csharp
using Horizon;
```

The easiest starting point is the static `Info` class. It requires a generic parameter. The generic is the type you want to query for info.

```csharp
Info<T>
```

A static constructor will be loaded, caching the information about this type once. At this point, the execution trees won't be compiled just yet, to keep the hit as small as possible. A simple [`GetMembers`](https://msdn.microsoft.com/en-us/library/k2w5ey1e.aspx) call is launched to get all the available info for the current type. This is parsed and converted to metadata Horizon can understand and work with.

Imagine we want to resolve the value of the field `_randomField`.

```csharp
var @class = new ClassWithFields();
var value = Info<T>.GetField(@class, "_randomField");
```

The field getter will only now be compiled and cached. All the other getters will be left untouched. (In other words, they're lazy compiled).

A generic type is not always a possibility. Horizon can also auto-resolve using an existing instance, instead. Continuing our previous example, it would look like this.

```csharp
var value = Info.GetField(@class, "_randomField");
```

For ease of use, it can also be invoked as an extension method.

```csharp
var value = @class.GetField("_randomField");
```

For obvious reasons, auto-resolving the type will have a small (almost non-existing) overhead compared to the generic call.

### Extended Info

Extended info is also available, which can be found in a static subclass.

```csharp
Info<T>.Extended
```

This static class contains a list of all the metadata Horizon is using for that type. (Methods, properties, constructors, etc)

Just like the normal `Info` class, the `Extended` class allows you to auto-resolve the type.

```csharp
Info.Extended.Fields(@class);
```

This class also allows you to pass a `Type` instead of an instance, in case you don't have one yet.

```csharp
Info.Extended.Fields(typeof (ClassWithFields));
```

Of course, this caller will have an added performance hit since it will try to resolve the metadata through the normal handler. This way it is shared between the two. The expression tree to resolve the data will be cached, so it will be a one-time hit.

### DLR

Horizon supports .NET's DLR. This means a lot of the calls on the `Info` class have an overload that accept an implementation of the [`CallSiteBinder`](https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callsitebinder.aspx).

In a nutshell, this means it's very easy to plug `Horizon` into any of your projects that uses [`DynamicObject`](https://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.aspx) implementations.


### The real fancy stuff

For simple calls, like getting values from fields and properties, an `Expression Tree` will be compiled and cached. After compilation, the execution hit is barely slower than making the actual call.

For more advanced calls, like methods that have named arguments and parameters with default values, a `CallSiteBinder` will be constructed, just like the DLR does! That makes it just as snappy as working with a regular dynamic object.

Horizon is smart enough to find the correct overload to use when calling a method, even if it means having to implicitely cast an instance first. When calling a method that requires more parameters than supplied, it will check for default values first. Named arguments are also supported, so it's possible to swap instances around, just like in regular C# code.

### Actually calling a method...

...Would look a little like this for a method without parameters (or one that has all defaultValues filled in):

```csharp
var value = @class.Call("MyMethod");
```

Or like this, in case you pass values:

```csharp
var value = @class.Call("MyMethodWithParameters", 1, "one", new OtherParameterClass());
```

When passing the `CallSiteBinder`, it will use named arguments as well. This is a sample that could be used when overriding `DynamicObject`.		

```csharp
public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)		
		
  object output;		
  if (_realInstance.TryCall(binder, args, out output))		
  {		
    result = output;		
    return true;		
  }		
		
  result = null;		
  return false;		
}		
		
```		
		
Horizon will resolve the passed arguments from the binder and will call the correct overload. If the method returns a value, it will be passed as well. If not, it will pass `null`.
