using System;

namespace Horizon
{
    partial interface IEventCaller : ICaller
    {
        void Raise(dynamic instance, params dynamic[] arguments);
        void Add(dynamic instance, params Delegate[] handlers);
        void Remove(dynamic instance, params Delegate[] handlers);
    }
}