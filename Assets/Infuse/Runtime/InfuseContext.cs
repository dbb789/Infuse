namespace Infuse
{
    public interface InfuseContext
    {
        void Infuse(object instance, bool defuseOnDestroy = true);
        void Defuse(object instance);
    }
}
