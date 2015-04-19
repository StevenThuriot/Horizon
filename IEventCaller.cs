using System;

namespace Horizon
{
    partial interface IEventCaller : ICaller
    {
        void Raise(dynamic instance, params dynamic[] arguments);
        void Add(dynamic instance, params Delegate[] handlers);
        void Remove(dynamic instance, params Delegate[] handlers);

        /// <summary>
        /// The default .NET compiler will not automatically create a Raise Method. 
        /// In this case, this property will be false and call invocation will be infered to the <see cref="NullCaller"/>.
        /// </summary>
        bool CanRaise { get; }
    }
}