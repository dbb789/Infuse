using System.Collections.Generic;
using System.Reflection;

namespace Infuse
{
    public interface IOnInfuseFuncBuilder
    {
        void AddMethod(MethodInfo methodInfo);
        OnInfuseFunc Build();
    }
}
