namespace Infuse.TypeInfo
{
    public class InfuseTypeEntry
    {
        public InfuseTypeInfo TypeInfo { get; private set; }
        public bool Resolved { get; set; }

        public InfuseTypeEntry(InfuseTypeInfo typeInfo)
        {
            TypeInfo = typeInfo;
            Resolved = false;
        }
    }
}
