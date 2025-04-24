using Infuse.Common;

namespace Infuse.Collections
{
    public abstract class ServiceContainer<T> : ServiceContainer where T : class
    {
        public override void Register(object instance)
        {
            if (instance is T tInstance)
            {
                Register(tInstance);
            }
            else
            {
                throw new InfuseException($"Type {instance.GetType()} is not assignable from {typeof(T)}.");
            }
        }

        public override void Unregister(object instance)
        {
            if (instance is T tInstance)
            {
                Unregister(tInstance);
            }
            else
            {
                throw new InfuseException($"Type {instance.GetType()} is not assignable from {typeof(T)}.");
            }
        }

        public abstract void Register(T instance);
        public abstract void Unregister(T instance);
    }

    public abstract class ServiceContainer
    {
        public abstract bool Populated { get; }
        
        public abstract void Register(object instance);
        public abstract void Unregister(object instance);
    }
}
