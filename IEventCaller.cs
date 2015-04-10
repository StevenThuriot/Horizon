using System;
using System.Collections.Generic;

namespace Horizon
{
    partial interface IEventCaller : ICaller
    {
        void Raise<T>(T instance, params dynamic[] arguments);
        void Add<T>(T instance, params Delegate[] handlers);
        void Remove<T>(T instance, params Delegate[] handlers);
    }
}