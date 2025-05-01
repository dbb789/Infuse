using System;

namespace Infuse
{
    public interface IFixedUpdateEvent
    {
        void Add(object self, Action updateFunc);
        void Remove(object self);
    }
}
