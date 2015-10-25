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

A static constructor will be loaded at this point, caching the information about this type once. At this point, the execution trees won't be compiled just yet, to keep the hit as small as possible. A simple [`GetMembers`](https://msdn.microsoft.com/en-us/library/k2w5ey1e.aspx) call is launched to get all the available info for the current type. This is parsed and converted to metadata Horizon can understand and work with.
