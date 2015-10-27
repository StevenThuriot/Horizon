---
title: "contribute"
bg: green
color: white
fa-icon: code
---

## Getting Started with Horizon

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

Extended info is also available, which can be found in a static subclass.

```csharp
Info<T>.Extended
```

This static class contains a list of all the metadata Horizon is using for that type. (Methods, properties, constructors, etc)
