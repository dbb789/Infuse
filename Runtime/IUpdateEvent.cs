using System;

namespace Infuse
{
    public interface IUpdateEvent
    {
        void Add(object self, Action updateFunc);
        void Remove(object self);
    }
}
