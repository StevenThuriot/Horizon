using System.Reflection;

namespace Horizon
{
    partial interface IConstructorCaller : ICaller
    {
        ConstructorInfo ConstructorInfo { get; }
    }
}