using System;

namespace DBB.Infuse
{
    public struct InfuseTarget
    {
        public object obj { get; private set; }
        public Action onInfuseBegin { get; private set; }
        public Action onInfuseEnd { get; private set; }

        public InfuseTarget(object obj,
                            Action onInfuseBegin,
                            Action onInfuseEnd)
        {
            this.obj = obj;
            this.onInfuseBegin = onInfuseBegin;
            this.onInfuseEnd = onInfuseEnd;
        }
    }
}
