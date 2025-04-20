namespace Infuse
{
    public interface IInfuseCompletionHandler
    {
        void OnInfuseCompleted(InfuseType infuseType, object instance);
    }
}
