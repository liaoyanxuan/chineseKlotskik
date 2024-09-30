using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sw.game.evt;

namespace sw.util
{
    public class TimeCtrl
    {
        static private TimeCtrl _instance;

        public static TimeCtrl Instance
        {
            get { 
                if(_instance==null)
                {
                    _instance = new TimeCtrl();
                    //GameObject go = GameObject.Find("TimeCtrl");
                    //if(go==null)
                    //{
                    //    go = new GameObject("TimeCtrl");
                    //    DontDestroyOnLoad(go);
                    //}
                    //_instance = go.GetComponent<TimeCtrl>();
                    //if(_instance == null)
                    //{
                    //    _instance = go.AddComponent<TimeCtrl>();
                    //}
                }
                return TimeCtrl._instance; 
            }
        }

        //static private bool isDestroy = false;

        #region 变量
        public delegate void CompleteHandler(double tid);
        public delegate void EveryHandle(double tid,double leftTime);

        private Dictionary<double,CompleteHandler> completeHandles;
        private Dictionary<double,double> completeTimes;
        private Dictionary<double, EveryHandle> everySecondHandles;
        private Dictionary<double, EveryHandle> everyMillisecondHandles;
        private Dictionary<double, EveryHandle> everyMinuteHandles;
        private Dictionary<double, double> everyHandleTimesMax;
        private Dictionary<double, double> everyHandleTimes;

        private Dictionary<double,TimeCtrlData> addList;

        private double timeDifference = 0;
        private double lastTime = 0;
        private double lastMillisecond = 0;
        private double lastMinute = 0;
        private double timeId;
        private bool isInit = false;
        private bool isSetServerTime = false;
        private double frameTime = 0;
        private double localFrameTime = 0;
        private List<double> delList;

        private double updateId = -1;

        #endregion

        public TimeCtrl()
        {
            //AddUpdate();
        }

        static public void Tick()
        {
            if(_instance==null)
            {
                return;
            }
            _instance.OnUpdate();
        }

        //private void AddUpdate()
        //{
        //    RemoveUpdate();
        //    updateId = EventDispatcher.AddEventListener(UIEventType.LUA_UPDATE, OnUpdate);
        //}

        //private void RemoveUpdate()
        //{
        //    if(updateId>=0)
        //    {
        //        EventDispatcher.RemoveEventListener(UIEventType.LUA_UPDATE, updateId);
        //        updateId = -1;
        //    }
        //}

        #region 生命周期
        // Use this for initialization
        //void Start()
        //{

        //}

        // Update is called once per frame
        void OnUpdate()
        {
            CheckAdd();
            CheckDel();
            CheckTime();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取当前系统时间 毫秒
        /// </summary>
        /// <returns></returns>
        public double GetSysTime()
        {
            return TimeHelper.GetSystemTime();
        }

        public double GetUnityTime()
        {
            return Time.realtimeSinceStartup * 1000;
        }

        /// <summary>
        /// 获取当前服务器时间(秒)
        /// </summary>
        /// <returns></returns>
        public double GetNowTime()
        {
            CheckInit();
            return (GetSysTime() + timeDifference) / 1000;
        }

        /// <summary>
        /// 获取当前服务器时间(毫秒)
        /// </summary>
        /// <returns></returns>
        public double GetNowTimeMs()
        {
            CheckInit();
            return GetSysTime() + timeDifference;
        }

        /// <summary>
        /// 设置服务器时间
        /// </summary>
        /// <param name="t"></param>
        public void SetServerTime(double t)
        {
            WriteServerTime(t);
        }

        /// <summary>
        /// 获取当前帧的服务器时间
        /// </summary>
        /// <returns></returns>
        public double GetFrameTime()
        {
            CheckInit();
            return frameTime;
        }

        /// <summary>
        /// 设置deadline时间点（秒）事件
        /// </summary>
        /// <param name="deadLine"></param>
        /// <param name="completeHandle"></param>
        /// <param name="everyHandle"></param>setdeadlinefun
        public double SetDeadLine(double deadLine,CompleteHandler completeHandle,EveryHandle everyHandle = null)
        {
            return AddDeadLine(deadLine * 1000, completeHandle, everyHandle);
        }

        /// <summary>
        /// 设置deadline时间点（秒）事件，传入是字符串
        /// </summary>
        /// <param name="deadLineStr"></param>
        /// <param name="completeHandle"></param>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetDeadLineStr(string deadLineStr, CompleteHandler completeHandle, EveryHandle everyHandle = null)
        {
            return AddDeadLine(double.Parse(deadLineStr) * 1000, completeHandle, everyHandle);
        }

        /// <summary>
        /// 设置deadline时间点（毫秒）事件
        /// </summary>
        /// <param name="deadLine"></param>
        /// <param name="completeHandle"></param>
        /// <param name="everyHandle"></param>
        public double SetDeadLineMs(double deadLine,CompleteHandler completeHandle,EveryHandle everyHandle = null)
        {
            return AddDeadLine(deadLine, completeHandle, everyHandle);
        }

        /// <summary>
        /// 设置deadline时间点（毫秒）事件，传入的时间是字符串
        /// </summary>
        /// <param name="deadLineStr"></param>
        /// <param name="completeHandle"></param>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetDeadLineMsStr(string deadLineStr, CompleteHandler completeHandle, EveryHandle everyHandle = null)
        {
            return AddDeadLine(double.Parse(deadLineStr), completeHandle, everyHandle);
        }

        /// <summary>
        /// 设置倒计时，单位秒
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="completeHandle"></param>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetCountDown(double sec,CompleteHandler completeHandle,EveryHandle everyHandle = null)
        {
            return AddCountDown(sec, completeHandle, everyHandle);
        }

        /// <summary>
        /// 设置倒计时，单位毫秒
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="completeHandle"></param>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetCountDownMs(double sec,CompleteHandler completeHandle,EveryHandle everyHandle = null)
        {
            return AddCountDownMs(sec, completeHandle, everyHandle);
        }

        /// <summary>
        /// 设置每秒执行
        /// </summary>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetEverySecond(EveryHandle everyHandle)
        {
            return SetEverySecondByTime(everyHandle, 1);
        }

        /// <summary>
        /// 设置每多少秒执行
        /// </summary>
        /// <param name="everyHandle"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SetEverySecondByTime(EveryHandle everyHandle,double t)
        {
            return AddSecond(everyHandle, t);
        }

        /// <summary>
        /// 设置每毫秒执行（最小执行频率受帧率影响）
        /// </summary>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetEveryMs(EveryHandle everyHandle)
        {
            return SetEveryMsByTime(everyHandle, 1);
        }

        /// <summary>
        /// 设置每多少毫秒执行(最小执行频率受帧率影响)
        /// </summary>
        /// <param name="everyHandle"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SetEveryMsByTime(EveryHandle everyHandle,double t)
        {
            return AddMs(everyHandle, t);
        }

        /// <summary>
        /// 设置每分钟执行
        /// </summary>
        /// <param name="everyHandle"></param>
        /// <returns></returns>
        public double SetEveryMinute(EveryHandle everyHandle)
        {
            return SetEveryMinuteByTime(everyHandle, 1);
        }


        /// <summary>
        /// 设置每多少分钟执行
        /// </summary>
        /// <param name="everyHandle"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SetEveryMinuteByTime(EveryHandle everyHandle,double t)
        {
            return AddMinute(everyHandle, t);
        }


        /// <summary>
        /// 获取 (当前时间+指定时间)
        /// </summary>
        /// <param 增加时间="timeStamp"></param>
        /// <returns>返回所需时间</returns>
        public string GetAddTime(string timeStamp)
        {
            System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            System.TimeSpan toNow = new System.TimeSpan(lTime);
            System.DateTime dtResult = dtStart.Add(toNow);
            string timerStr = dtResult.ToShortDateString().ToString();
            return timerStr;
        }

        public void RemoveTime(double id)
        {
            if(delList == null)
            {
                delList = new List<double>();
            }
            delList.Add(id);
        }

        public void Clear()
        {
            Init();
        }

        #endregion

        private void CheckInit()
        {
            if(isInit==false)
            {
                Init();
            }
        }

        private void Init()
        {
            completeHandles = new Dictionary<double, CompleteHandler>();
            completeTimes = new Dictionary<double, double>();
            everySecondHandles = new Dictionary<double, EveryHandle>();
            everyMillisecondHandles = new Dictionary<double,EveryHandle>();
            everyMinuteHandles = new Dictionary<double, EveryHandle>();
            everyHandleTimesMax = new Dictionary<double, double>();
            everyHandleTimes = new Dictionary<double, double>();
            delList = new List<double>();
            timeDifference = 0;
            lastTime = 0;
            lastMillisecond = 0;
            lastMinute = 0;
            timeId = 0;
            isInit = true;
            isSetServerTime = false;
            frameTime = GetNowTimeMs();

        }

        private void CheckTime()
        {
            frameTime = GetNowTimeMs();
            localFrameTime = GetUnityTime();
            CheckComplete(frameTime);
            CheckSecond(frameTime, localFrameTime);
            CheckMs(frameTime, localFrameTime);
            CheckMinute(frameTime, localFrameTime);
            
        }

        private void CheckAdd()
        {
            if(addList == null||addList.Count==0)
            {
                return;
            }

            foreach(KeyValuePair<double,TimeCtrlData>item in addList)
            {
                AddOneTimeEvent(item.Value);
            }
            addList = new Dictionary<double, TimeCtrlData>();
        }

        private void CheckDel()
        {
            if(delList==null||delList.Count==0)
            {
                return;
            }

            for(int i = 0;i<delList.Count;i++)
            {
                Remove(delList[i]);
            }
            delList = new List<double>();
        }

        #region 实现方法

        private void AddOneTimeEvent(TimeCtrlData data)
        {
            int eventType = data.type;
            if(eventType == TimeEventType.deadLine)
            {
                AddDeadLineFun(data);
            }
            else if(eventType == TimeEventType.countDown)
            {
                AddCountDownFun(data);
            }
            else if(eventType == TimeEventType.countDownMs)
            {
                AddCountDownMsFun(data);
            }
            else if(eventType==TimeEventType.sec)
            {
                AddSecondFun(data);
            }
            else if(eventType == TimeEventType.ms)
            {
                AddMsFun(data);
            }
            else if(eventType == TimeEventType.minute)
            {
                AddMinuteFun(data);
            }
        }

        private bool CheckInDelList(double id)
        {
            if(delList==null||delList.Count == 0)
            {
                return false;
            }
            if(delList.IndexOf(id)>=0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<double> checkCompleteRemoveList=new List<double>();
        private double checkCompleteCt = -1;
        private void CheckComplete(double t)
        {
            if(completeHandles == null ||completeHandles.Count == 0)
            {
                return;
            }
            //List<double> removeList = new List<double>();
            checkCompleteRemoveList.Clear();
            checkCompleteCt = -1;
            foreach(KeyValuePair<double,CompleteHandler>item in completeHandles)
            {
                try
                {
                    if (CheckInDelList(item.Key) == true)
                    {
                        continue;
                    }
                    if (completeTimes.ContainsKey(item.Key))
                    {
                        checkCompleteCt = completeTimes[item.Key];
                        if (checkCompleteCt < t)
                        {
                            item.Value(item.Key);
                            checkCompleteRemoveList.Add(item.Key);
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    LoggerHelper.Error(ex.Message);
                }
            }

            if (checkCompleteRemoveList.Count > 0)
            {
                for (int i = 0; i < checkCompleteRemoveList.Count; i++)
                {
                    Remove(checkCompleteRemoveList[i]);
                }
            }


        }

        private void CheckEvery(double tid,EveryHandle handle,double leftTime,double addtime)
        {
            double tInd = 0;
            if(everyHandleTimes.ContainsKey(tid))
            {
                tInd = everyHandleTimes[tid];
            }
            double tMax = 1;
            if(everyHandleTimesMax.ContainsKey(tid))
            {
                tMax = everyHandleTimesMax[tid];
            }
            tInd = tInd + addtime;
            if(tInd>=tMax)
            {
                if(everyHandleTimes.ContainsKey(tid))
                {
                    everyHandleTimes.Remove(tid);
                }
                leftTime = System.Math.Floor(leftTime);
                handle(tid, leftTime);
            }
            else
            {
                if(everyHandleTimes.ContainsKey(tid))
                {
                    everyHandleTimes[tid] = tInd;
                }
                else
                {
                    everyHandleTimes.Add(tid, tInd);
                }
            }

        }

        private void CheckSecond(double t,double lt)
        {
            if(everySecondHandles==null||everySecondHandles.Count==0)
            {
                return;
            }
            double passTime = lt - lastTime;
            if(passTime>=1000)
            {
                double leftTime = -1;
                foreach(KeyValuePair<double,EveryHandle>item in everySecondHandles)
                {
                    if (CheckInDelList(item.Key) == true)
                    {
                        continue;
                    }
                    leftTime = 0;
                    if(completeTimes.ContainsKey(item.Key))
                    {
                        leftTime = (completeTimes[item.Key] - t) / 1000;
                    }
                    try
                    {
                        CheckEvery(item.Key, item.Value, leftTime, passTime / 1000);
                    }
                    catch(System.Exception ex)
                    {
                        LoggerHelper.Error(ex.Message);
                    }
                    
                }
                lastTime = lt;
            }
        }

        private void CheckMs(double t,double lt)
        {
            if(everyMillisecondHandles==null||everyMillisecondHandles.Count == 0)
            {
                return;
            }
            double passTime = lt - lastMillisecond;
            if(passTime>0)
            {
                double leftTime = 0;
                foreach(KeyValuePair<double,EveryHandle>item in everyMillisecondHandles)
                {
                    if (CheckInDelList(item.Key) == true)
                    {
                        continue;
                    }
                    leftTime = 0;
                    if(completeTimes.ContainsKey(item.Key))
                    {
                        leftTime = completeTimes[item.Key]-t;
                    }
                    try
                    {
                        CheckEvery(item.Key, item.Value, leftTime, passTime);
                    }
                    catch(System.Exception ex)
                    {
                        LoggerHelper.Error(ex.Message);
                    }
                    
                }
                lastMillisecond = lt;
            }
        }

        private void CheckMinute(double t,double lt)
        {
            if(everyMinuteHandles==null||everyMinuteHandles.Count ==0)
            {
                return;
            }
            double passTime = lt - lastMinute;
            if(passTime>=60000)
            {
                double leftTime = 0;
                foreach(KeyValuePair<double,EveryHandle>item in everyMinuteHandles)
                {
                    if (CheckInDelList(item.Key) == true)
                    {
                        continue;
                    }
                    leftTime = 0;
                    if(completeTimes.ContainsKey(item.Key))
                    {
                        leftTime = (completeTimes[item.Key] - t) / 60000;
                    }
                    try
                    {
                        CheckEvery(item.Key, item.Value, leftTime, passTime / 60000);
                    }
                    catch(System.Exception ex)
                    {
                        LoggerHelper.Error(ex.Message);
                    }
                    
                }
                lastMinute = lt;
            }
        }


        private void WriteServerTime(double t)
        {
            CheckInit();
            if (isSetServerTime!=true)
            {
                lastTime = 0;
                lastMinute = 0;
                lastMillisecond = 0;
                isSetServerTime = true;
            }
            double now = GetSysTime();
            timeDifference = t - now;
            frameTime = GetNowTimeMs();
        }

        private double GetTimeId()
        {
            timeId++;
            return timeId;
        }



        private void Add2AddList(TimeCtrlData data)
        {
            if(addList==null)
            {
                addList = new Dictionary<double, TimeCtrlData>();
            }
            addList.Add(data.id, data);
        }

        private double AddDeadLine(double deadLine,CompleteHandler completeHandle,EveryHandle everyHandle = null)
        {
            CheckInit();
            if(deadLine<0)
            {
                return -1;
            }
            if(completeHandle==null)
            {
                return -1;
            }
            double tid = GetTimeId();
            TimeCtrlData data = new TimeCtrlData();
            data.id = tid;
            data.type = TimeEventType.deadLine;
            data.deadLine = deadLine;
            data.completeHandle = completeHandle;
            if(everyHandle!=null)
            {
                data.everyHandle = everyHandle;
            }
            Add2AddList(data);
            return tid;
        }

        private double AddCountDown(double sec,CompleteHandler completeHandle,EveryHandle everyHandle = null)
        {
            CheckInit();
            if(completeHandle == null)
            {
                return -1;
            }
            double tid = GetTimeId();
            double now = frameTime;
            double deadLine = now + sec * 1000;
            TimeCtrlData data = new TimeCtrlData();
            data.id = tid;
            data.type = TimeEventType.countDown;
            data.deadLine = deadLine;
            data.completeHandle = completeHandle;
            if(everyHandle!=null)
            {
                data.everyHandle = everyHandle;
            }
            Add2AddList(data);
            return tid;
        }

        private double AddCountDownMs(double sec, CompleteHandler completeHandle, EveryHandle everyHandle = null)
        {
            CheckInit();
            if (completeHandle == null)
            {
                return -1;
            }
            double tid = GetTimeId();
            double now = frameTime;
            double deadLine = now + sec;
            TimeCtrlData data = new TimeCtrlData();
            data.id = tid;
            data.type = TimeEventType.countDownMs;
            data.deadLine = deadLine;
            data.completeHandle = completeHandle;
            if (everyHandle != null)
            {
                data.everyHandle = everyHandle;
            }
            Add2AddList(data);
            return tid;
        }

        private double AddSecond(EveryHandle everyHandle,double t)
        {
            CheckInit();
            if (everyHandle == null)
            {
                return -1;
            }
            double tid = GetTimeId();
            TimeCtrlData data = new TimeCtrlData();
            data.type = TimeEventType.sec;
            data.id = tid;
            data.everyHandle = everyHandle;
            data.t = t;
            Add2AddList(data);
            return tid;
        }

        private double AddMs(EveryHandle everyHandle,double t)
        {
            CheckInit();
            if(everyHandle == null)
            {
                return 1;
            }
            double tid = GetTimeId();
            TimeCtrlData data = new TimeCtrlData();
            data.type = TimeEventType.ms;
            data.id = tid;
            data.everyHandle = everyHandle;
            data.t = t;
            Add2AddList(data);
            return tid;
        }

        private double AddMinute(EveryHandle everyHandle,double t)
        {
            CheckInit();
            if(everyHandle == null)
            {
                return -1;
            }
            double tid = GetTimeId();
            TimeCtrlData data = new TimeCtrlData();
            data.type = TimeEventType.minute;
            data.id = tid;
            data.everyHandle = everyHandle;
            data.t = t;
            Add2AddList(data);
            return tid;
        }

        private void AddDeadLineFun(TimeCtrlData data)
        {
            double tid = data.id;
            if(completeHandles.ContainsKey(tid))
            {
                completeHandles[tid] = data.completeHandle;
            }
            else
            {
                completeHandles.Add(tid, data.completeHandle);
            }

            if(completeTimes.ContainsKey(tid))
            {
                completeTimes[tid] = data.deadLine;
            }
            else
            {
                completeTimes.Add(tid, data.deadLine);
            }

            if(data.everyHandle!=null)
            {
                if(everySecondHandles.ContainsKey(tid))
                {
                    everySecondHandles[tid] = data.everyHandle;
                }
                else
                {
                    everySecondHandles.Add(tid, data.everyHandle);
                }
            }
        }

        private void AddCountDownFun(TimeCtrlData data)
        {
            double tid = data.id;
            if(completeHandles.ContainsKey(tid))
            {
                completeHandles[tid] = data.completeHandle;
            }
            else
            {
                completeHandles.Add(tid, data.completeHandle);
            }

            if(completeTimes.ContainsKey(tid))
            {
                completeTimes[tid] = data.deadLine;
            }
            else
            {
                completeTimes.Add(tid, data.deadLine);
            }

            if(data.everyHandle !=null)
            {
                if(everySecondHandles.ContainsKey(tid))
                {
                    everySecondHandles[tid] = data.everyHandle;
                }
                else
                {
                    everySecondHandles.Add(tid, data.everyHandle);
                }
            }
        }

        private void AddCountDownMsFun(TimeCtrlData data)
        {
            double tid = data.id;
            if(completeHandles.ContainsKey(tid))
            {
                completeHandles[tid] = data.completeHandle;
            }
            else
            {
                completeHandles.Add(tid, data.completeHandle);
            }

            if(completeTimes.ContainsKey(tid))
            {
                completeTimes[tid] = data.deadLine;
            }
            else
            {
                completeTimes.Add(tid, data.deadLine);
            }

            if(data.everyHandle!=null)
            {
                if(everyMillisecondHandles.ContainsKey(tid))
                {
                    everyMillisecondHandles[tid] = data.everyHandle;
                }
                else
                {
                    everyMillisecondHandles.Add(tid, data.everyHandle);
                }
            }
        }

        private void AddSecondFun(TimeCtrlData data)
        {
            double tid = data.id;
            if(everySecondHandles.ContainsKey(tid))
            {
                everySecondHandles[tid] = data.everyHandle;
            }
            else
            {
                everySecondHandles.Add(tid, data.everyHandle);
            }

            if(everyHandleTimes.ContainsKey(tid))
            {
                everyHandleTimes[tid] = 0;
            }
            else
            {
                everyHandleTimes.Add(tid, 0);
            }

            if(everyHandleTimesMax.ContainsKey(tid))
            {
                everyHandleTimesMax[tid] = data.t;
            }
            else
            {
                everyHandleTimesMax.Add(tid, data.t);
            }
        }

        private void AddMsFun(TimeCtrlData data)
        {
            double tid = data.id;
            if(everyMillisecondHandles.ContainsKey(tid))
            {
                everyMillisecondHandles[tid] = data.everyHandle;
            }
            else
            {
                everyMillisecondHandles.Add(tid, data.everyHandle);
            }
            if(everyHandleTimes.ContainsKey(tid))
            {
                everyHandleTimes[tid] = 0;
            }
            else
            {
                everyHandleTimes.Add(tid, 0);
            }

            if(everyHandleTimesMax.ContainsKey(tid))
            {
                everyHandleTimesMax[tid] = data.t;
            }
            else
            {
                everyHandleTimesMax.Add(tid, data.t);
            }
        }

        private void AddMinuteFun(TimeCtrlData data)
        {
            double tid = data.id;
            if(everyMinuteHandles.ContainsKey(tid))
            {
                everyMinuteHandles[tid] = data.everyHandle;
            }
            else
            {
                everyMinuteHandles.Add(tid, data.everyHandle);
            }

            if(everyHandleTimes.ContainsKey(tid))
            {
                everyHandleTimes[tid] = 0;
            }
            else
            {
                everyHandleTimes.Add(tid, 0);
            }

            if(everyHandleTimesMax.ContainsKey(tid))
            {
                everyHandleTimesMax[tid] = data.t;
            }
            else
            {
                everyHandleTimesMax.Add(tid, data.t);
            }
        }

        private void RemoveFromAddList(double id)
        {
            if(addList!=null&&addList.ContainsKey(id))
            {
                addList.Remove(id);
            }
        }

        private void Remove(double id)
        {
            if(completeHandles.ContainsKey(id))
            {
                completeHandles.Remove(id);
            }

            if(completeTimes.ContainsKey(id))
            {
                completeTimes.Remove(id);
            }

            if(everySecondHandles.ContainsKey(id))
            {
                everySecondHandles.Remove(id);
            }

            if(everyMillisecondHandles.ContainsKey(id))
            {
                everyMillisecondHandles.Remove(id);
            }

            if(everyMinuteHandles.ContainsKey(id))
            {
                everyMinuteHandles.Remove(id);
            }

            if(everyHandleTimes.ContainsKey(id))
            {
                everyHandleTimes.Remove(id);
            }

            if(everyHandleTimesMax.ContainsKey(id))
            {
                everyHandleTimesMax.Remove(id);
            }

            RemoveFromAddList(id);
            
        }

        private void OnDestroy()
        {
            //isDestroy = true;
        }

        static public void DisposeClass()
        {
            if(_instance!=null)
            {
                //_instance.RemoveUpdate();
                _instance.Clear();                
                _instance = null;
            }
        }

        #endregion 
    }

    public class TimeCtrlData
    {
        public int type;
        public double deadLine;
        public TimeCtrl.CompleteHandler completeHandle;
        public TimeCtrl.EveryHandle everyHandle;
        public double id;
        public double t = 1;

    }

    public class TimeEventType
    {
        public const int deadLine = 0;
        public const int countDown = 1;
        public const int countDownMs = 2;
        public const int sec = 3;
        public const int ms = 4;
        public const int minute = 5;
        
    }
}
