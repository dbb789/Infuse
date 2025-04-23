namespace Infuse
{
    public interface InfuseContext
    {
        void Register(object instance, bool unregisterOnDestroy = true);
        void Unregister(object instance);

        void RegisterService<TServiceType>(object instance) where TServiceType : class;
        void UnregisterService<TServiceType>(object instance) where TServiceType : class;
    }
}
