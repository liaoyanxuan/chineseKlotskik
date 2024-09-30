namespace sw.util
{
    using System;

    internal class TimerData : AbsTimerData
    {
        private System.Action m_action;

        public override void DoAction()
        {
           
                this.m_action();
           
        }
        public override string GetDebugInfo()
        {
            return m_action == null ? "null action" : ("target:" + m_action.Target + ",method:" + m_action.Method);

        }
        public override Delegate Action
        {
            get
            {
                return this.m_action;
            }
            set
            {
                this.m_action = value as System.Action;
            }
        }
    }
    internal class TimerData<T> : AbsTimerData
    {
        private Action<T> m_action;
        private T m_arg1;

        public override void DoAction()
        {
           
            this.m_action(this.m_arg1);
        }
        public override string GetDebugInfo()
        {
            return m_action == null ? "null action" : ("target:" + m_action.Target + ",method:" + m_action.Method);

        }
        public override Delegate Action
        {
            get
            {
                return this.m_action;
            }
            set
            {
                this.m_action = value as Action<T>;
            }
        }

        public T Arg1
        {
            get
            {
                return this.m_arg1;
            }
            set
            {
                this.m_arg1 = value;
            }
        }
    }
    internal class TimerData<T, U> : AbsTimerData
    {
        private Action<T, U> m_action;
        private T m_arg1;
        private U m_arg2;

        public override void DoAction()
        {
            this.m_action(this.m_arg1, this.m_arg2);
        }

        public override Delegate Action
        {
            get
            {
                return this.m_action;
            }
            set
            {
                this.m_action = value as Action<T, U>;
            }
        }

        public T Arg1
        {
            get
            {
                return this.m_arg1;
            }
            set
            {
                this.m_arg1 = value;
            }
        }

        public U Arg2
        {
            get
            {
                return this.m_arg2;
            }
            set
            {
                this.m_arg2 = value;
            }
        }

        public override string GetDebugInfo()
        {
            return m_action==null?"null action": ("target:" + m_action.Target + ",method:" + m_action.Method);

        }
    }
    internal class TimerData<T, U, V> : AbsTimerData
    {
        private Action<T, U, V> m_action;
        private T m_arg1;
        private U m_arg2;
        private V m_arg3;

        public override void DoAction()
        {
            this.m_action(this.m_arg1, this.m_arg2, this.m_arg3);
        }

        public override Delegate Action
        {
            get
            {
                return this.m_action;
            }
            set
            {
                this.m_action = value as Action<T, U, V>;
            }
        }

        public T Arg1
        {
            get
            {
                return this.m_arg1;
            }
            set
            {
                this.m_arg1 = value;
            }
        }
        public override string GetDebugInfo()
        {
            return m_action == null ? "null action" : ("target:" + m_action.Target + ",method:" + m_action.Method);

        }
        public U Arg2
        {
            get
            {
                return this.m_arg2;
            }
            set
            {
                this.m_arg2 = value;
            }
        }

        public V Arg3
        {
            get
            {
                return this.m_arg3;
            }
            set
            {
                this.m_arg3 = value;
            }
        }
    }
}

