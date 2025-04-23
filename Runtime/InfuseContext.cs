namespace Infuse
{
    public interface InfuseContext
    {
        void Infuse(object instance, bool defuseOnDestroy = true);
        void Defuse(object instance);

        void RegisterService<TServiceType>(object instance) where TServiceType : class;
        void UnregisterService<TServiceType>(object instance) where TServiceType : class;
    }
}
