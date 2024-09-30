namespace sw.util
{

    using sw.util;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using UnityEngine;

    public class TimerManager2
    {
        private static uint m_nNextTimerId;
        private static KeyedPriorityQueue<Delegate, AbsTimerData, ulong> m_queue = new KeyedPriorityQueue<Delegate, AbsTimerData, ulong>();
        private static List<AbsTimerData> m_nextCall = new List<AbsTimerData>();
        private static readonly object m_queueLock = new object();
        private static Stopwatch m_stopWatch = new Stopwatch();
        private static uint m_unTick;
        private TimerManager2()
        {
        }
        public static int getCurrentTime()
        {
            return (int)(Time.time * 1000);
        }
        public static string timerlistinfo()
        {
            StringBuilder s = new StringBuilder();
            foreach (AbsTimerData item in m_queue.Values)
            {
                s.Append("\n time: ").Append(item.NInterval).Append("\n   actionName: ").Append(item.Action != null ? item.Action.Method.Name : "null")
                    .Append("   \nDeclaringType: ").Append(item.Action.Method.DeclaringType);
            }
            return s.ToString();
        }

        private static uint AddTimer(AbsTimerData p)
        {
            lock (m_queueLock)
            {
                m_queue.Enqueue(p.Action, p, p.UnNextTick);
            }
            return p.NTimerId;
        }
       

        private static void AddNextFrame(AbsTimerData p)
        {
            lock (m_queueLock)
            {
                for (int i = 0; i < m_nextCall.Count; i++)
                {
                    if (m_nextCall[i].Action == p.Action)
                        return;
                }

                m_nextCall.Add(p);

            }
        }
        public static void CallNextFrame(Action handler)
        {
            TimerData p = GetTimerData<TimerData>(new TimerData(), 0, 0);
            p.Action = handler;
            AddNextFrame(p);

        }

        public static void CallNextFrame2(string desc,Action handler)
        {
#if ENABLE_PROFILER
            TimerData p = GetTimerData<TimerData>(new TimerData(), 0, 0);
            p.desc = desc;
            p.Action = handler;
            AddNextFrame(p);
#endif
        }

        public static void CallNextFrame<T>(Action<T> handler, T arg1)
        {
            TimerData<T> p = GetTimerData<TimerData<T>>(new TimerData<T>(), 0, 0);
            p.Action = handler;
            p.Arg1 = arg1;
            AddNextFrame(p);

        }
        public static void CallNextFrame<T, U>(Action<T> handler, T arg1, U arg2)
        {
            TimerData<T, U> p = GetTimerData<TimerData<T, U>>(new TimerData<T, U>(), 0, 0);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            AddNextFrame(p);
        }
        public static void CallNextFrame<T, U, V>(Action<T> handler, T arg1, U arg2, V arg3)
        {
            TimerData<T, U, V> p = GetTimerData<TimerData<T, U, V>>(new TimerData<T, U, V>(), 0, 0);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            p.Arg3 = arg3;
            AddNextFrame(p);
        }
        public static uint AddTimer(uint start, int interval, Action handler)
        {
            TimerData p = GetTimerData<TimerData>(new TimerData(), start, interval);
            p.Action = handler;
            return AddTimer(p);
        }
      
        public static uint AddTimer<T>(uint start, int interval, Action<T> handler, T arg1)
        {
            TimerData<T> p = GetTimerData<TimerData<T>>(new TimerData<T>(), start, interval);
            p.Action = handler;
            p.Arg1 = arg1;
            return AddTimer(p);
        }

        public static uint AddTimer<T, U>(uint start, int interval, Action<T, U> handler, T arg1, U arg2)
        {
            TimerData<T, U> p = GetTimerData<TimerData<T, U>>(new TimerData<T, U>(), start, interval);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            return AddTimer(p);
        }

        public static uint AddTimer<T, U, V>(uint start, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3)
        {
            TimerData<T, U, V> p = GetTimerData<TimerData<T, U, V>>(new TimerData<T, U, V>(), start, interval);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            p.Arg3 = arg3;
            return AddTimer(p);
        }
        public static void DelTimer<T, U, V>(Action<T, U, V> handler)
        {
            lock (m_queueLock)
            {

                m_queue.Remove(handler);
            }
        }
        public static void DelTimer<T, U>(Action<T, U> handler)
        {
            lock (m_queueLock)
            {

                m_queue.Remove(handler);
            }
        }
        public static void DelTimer<T>(Action<T> handler)
        {
            lock (m_queueLock)
            {

                m_queue.Remove(handler);
            }
        }
        public static void DelTimer(Action handler)
        {
            lock (m_queueLock)
            {

                m_queue.Remove(handler);
            }
        }
        private static T GetTimerData<T>(T p, uint start, int interval) where T : AbsTimerData
        {
            p.NInterval = interval;
            p.NTimerId = ++m_nNextTimerId;
            p.UnNextTick = (m_unTick + 1) + start;
            return p;
        }

        public static void Reset()
        {
            m_unTick = 0;
            m_nNextTimerId = 0;
            lock (m_queueLock)
            {
                while (m_queue.Count != 0)
                {
                    m_queue.Dequeue();
                }
               
            }
        }
        private static ulong prevTick;
        const long MAX_TICK_TIME = 20;
        public static void Tick()
        {
            m_unTick += (uint)m_stopWatch.ElapsedMilliseconds;
            m_stopWatch.Reset();
            m_stopWatch.Start();
            while (m_queue.Count != 0 && m_stopWatch.ElapsedMilliseconds < MAX_TICK_TIME)
            {
                AbsTimerData data;
                object obj2;
                lock ((obj2 = m_queueLock))
                {
                    data = m_queue.Peek();
                }
                if (m_unTick < data.UnNextTick)
                {
                    break;
                }
                lock ((obj2 = m_queueLock))
                {
                    m_queue.Dequeue();
                }
                //if (prevTick > data.UnNextTick)
                //    LoggerHelper.Debug("invalid tick:" + prevTick + "," + data.UnNextTick + "," + m_unTick);
                prevTick = data.UnNextTick;
                if (data.NInterval > 0)
                {
                    data.UnNextTick += (ulong)data.NInterval;
                    if(data.UnNextTick<m_unTick)
                        data.UnNextTick = m_unTick + (ulong)data.NInterval;
                    lock ((obj2 = m_queueLock))
                    {
                        m_queue.Enqueue(data.Action, data, data.UnNextTick);
                    }
                    try
                    {
#if ENABLE_PROFILER
 
                        //LuaUtil.BeginSample("timeraction1_" + data.NInterval + "_" + data.desc);
#endif
                        data.DoAction();
#if ENABLE_PROFILER
                        //LuaUtil.EndSample();
 
#endif
                    }
                   catch(Exception ex)
                    {
                        LoggerHelper.Except(ex,"do action error:" + ex.Message+",desc:"+data.GetDebugInfo());
                       //这里原来是catch到错误就把那个事件删掉，但一些全局运行的注册方法是不能删掉的，所以先注释掉
                        //m_queue.Remove(data.Action);
                    }
                    //LoggerHelper.Debug("do action:" + data.NTimerId);
                }
                else
                {
                    try
                    {
#if ENABLE_PROFILER
 
                       // LuaUtil.BeginSample("timeraction2_" + data.NInterval + "_" + data.desc);
                    
#endif
                        data.DoAction();

                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Error("do action error:" + ex.Message);
                        m_queue.Remove(data.Action);
                    }
                    finally
                    {
#if ENABLE_PROFILER

                       // LuaUtil.EndSample();
#endif
                    }
                }
            }
            for (int i = 0; i < m_nextCall.Count; i++)
            {
                try {
#if ENABLE_PROFILER
                    //LuaUtil.BeginSample("timeraction_next_" + m_nextCall[i].desc);

#endif
                    m_nextCall[i].DoAction();
                }
#if ENABLE_PROFILER
                    //  LuaUtil.EndSample();

#endif
                catch (Exception ex) {
                    LoggerHelper.Error("m_nextCall do action error:" + ex.Message);
                }
            }
            m_nextCall.Clear();
        }
         
        public static string getTimeFmt(uint second)
        {
            float hh = (second / 3600) >> 0;//把秒除得时间
            float mm = (second % 3600) / 60 >> 0;//把余数用于除得分钟
            float ss = (second % 3600) % 60 >> 0;//最后的余数直接就是秒钟
            string tss = ss < 10 ? "0" + ss : ss + "";
            string tmm = mm < 10 ? "0" + mm : mm + "";
            string thh = hh < 10 ? "0" + hh : hh + "";
            return thh + ":" + tmm + ":" + tss;
        }
        public static string getTimeFm(uint second)
        {
            float mm = (second % 3600) / 60 >> 0;//把余数用于除得分钟
            float ss = (second % 3600) % 60 >> 0;//最后的余数直接就是秒钟
            string tss = ss < 10 ? "0" + ss : ss + "";
            return mm + ":" + tss;
        }

        public static string getTimeStr(uint second)
        {
            string str = "";
            if (second < 3600)
            {
                str = ((int)second / 60) + "分钟";
            }
            else if (second >= 3600 && second < 86400)
            {
                str = ((int)second / 3600).ToString() + "小时";
            }
            else if (second >= 86400)
            {
                str = ((int)second / 86400) + "天";
            }
            return str;
        }
    }
}

