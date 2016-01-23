# Horizon

A .NET Fast Very Late Binding invoker

![Beyond the horizon
by agnidevi](https://cloud.githubusercontent.com/assets/544444/12533192/19b405d8-c228-11e5-9a12-2f6720e09e7e.jpg)

#### Basically…

### Reflection is hard… And dreadfully slow!

Horizon addresses these issues by supplying a **simple to use** API.

Underneath the covers, Horizon will create and cache **Expression Trees**. This means there will be a small *one time hit* which will be hardly noticeable. 

### From then on, Horizon will be **blazing fast**!

As if that wasn't enough, Horizon also supports the *DLR's Binders*, making it super easy to plug it into your very own *DynamicObject*s! Hooray!
