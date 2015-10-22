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

A static constructor will be loaded at this point, caching the information about this type once.
