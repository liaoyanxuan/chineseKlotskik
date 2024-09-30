namespace sw.util
{
    using System;

    internal abstract class AbsTimerData
    {
        private int m_nInterval;
        private uint m_nTimerId;
        private ulong m_unNextTick;
#if ENABLE_PROFILER
        public string desc;
#endif
        protected AbsTimerData()
        {
        }

        public abstract void DoAction();

        public abstract Delegate Action { get; set; }
        public abstract string GetDebugInfo();
        public int NInterval
        {
            get
            {
                return this.m_nInterval;
            }
            set
            {
                this.m_nInterval = value;
            }
        }

        public uint NTimerId
        {
            get
            {
                return this.m_nTimerId;
            }
            set
            {
                this.m_nTimerId = value;
            }
        }

        public ulong UnNextTick
        {
            get
            {
                return this.m_unNextTick;
            }
            set
            {
                this.m_unNextTick = value;
            }
        }
    }
}

