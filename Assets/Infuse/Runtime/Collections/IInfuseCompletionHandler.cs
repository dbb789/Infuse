namespace Infuse.Collections
{
    public interface IInfuseCompletionHandler
    {
        void OnInfuseCompleted(InfuseTypeInfo typeInfo, object instance);
    }
}
