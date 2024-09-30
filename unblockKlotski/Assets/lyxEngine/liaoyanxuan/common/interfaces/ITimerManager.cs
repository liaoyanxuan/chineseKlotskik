using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace liaoyanxuan.common.interfaces
{
    public interface ITimerManager
    {
        uint AddTimer(uint start, int interval, Action handler);

        void DelTimer(Action handler);

        int getCurrentTime(); 
    }
}
