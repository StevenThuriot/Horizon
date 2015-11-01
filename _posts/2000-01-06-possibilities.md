---
title: "possibilities"
bg: darkorange
color: white
fa-icon: magic
---

So what can Horizon exactly do?

Horizon can:

- Get and Set field and property values
  - Both private as public, wether it's on a base class or not.
- Get and Set values and guess if it's a property or field.
  - In case you're not sure, or scared that the implementation in the third-party dll might change
- Add event handlers
- Remove them again!
- Raise events
- Get or Set indexers.
  - With one or multiple parameters
- Implicitely convert types
  - Or just check if it's possible.
- Create instances
  - With or without parameters.
- And most importantly: Call methods!
- With safe to use `Try` variants for each method, in case your input is incorrect. 
  - Returns a bool instead, with possibly an `out` parameter.
- Check if a field, property, event or method exists on the given type.
