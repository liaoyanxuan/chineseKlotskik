using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sw.game.evt;
using sw.util;
using System.Text;

public class EventDispatcher
{
    public delegate void EventBack(params object[] arg);

    private static Dictionary<string, Dictionary<double, EventBack>> _events = new Dictionary<string, Dictionary<double, EventBack>>(); 

    private static double _currentIndex = 0;
    
    private static List<EventMainThreadData> _mainThreadData = new List<EventMainThreadData>();

    //private static List<string> _mainThreadTypes = new List<string>();
    //private static List<object[]> __mainThreadArgs = new List<object[]>();

    private static bool isDispatching = false;

    private static Dictionary<double,string> delayRemoveList = new Dictionary<double,string>();

    private static List<EventAddData> delayAddList = new List<EventAddData>();

    private static List<DispatchEventData> delayDispatchList = new List<DispatchEventData>();

    private static int dispatchCount = 0;

    private static object lockobj = new object();

    private static int mainThreadDataLen = 0;

    private static EventMainThreadData mainThreadData;

    public static void OnUpdate()
    {
        lock (lockobj)
        {
            mainThreadDataLen = _mainThreadData.Count;  //_mainThreadTypes.Count;
            if (mainThreadDataLen == 0)
            {
                return;
            }
            double startTime = TimeCtrl.Instance.GetNowTimeMs();
            double usedTime = 0;
            double nowTime = 0;
            for (int i = 0; i < mainThreadDataLen; i++)
            {
                if (_mainThreadData.Count > 0)
                {
                    mainThreadData = _mainThreadData[0];
                    _mainThreadData.RemoveAt(0);
                    if (mainThreadData == null)
                    {
                        continue;
                    }
                    string type = mainThreadData.type; //_mainThreadTypes[i];
                    object[] arg = mainThreadData.param; //__mainThreadArgs[i];  
                    mainThreadData = null;
                    DispatchEvent(type, arg);
                    nowTime = TimeCtrl.Instance.GetNowTimeMs();
                    usedTime = nowTime - startTime;
                    if(usedTime>15)
                    {
                        break;
                    }
                }
            }
        }

        //_mainThreadTypes.Clear();
        //__mainThreadArgs.Clear();
    }

    /// <summary>
    /// 添加侦听到消息队列
    /// </summary>
    /// <param name="type"></param>
    /// <param name="func"></param>
	public static double AddEventListener(string type, EventBack func)
    {
    
        
        _currentIndex++;
        if(isDispatching)
        {
            Add2DelayAddList(type, _currentIndex, func);
        }
        else
        {
            AddEventFun(type, _currentIndex, func);
        }

        return _currentIndex;
    }

    static private Dictionary<double, EventBack> addEventfuncs = null;
    private static void AddEventFun(string type,double id,EventBack func)
    {
        //LoggerHelper.Debug("AddEventFun "+type);
        addEventfuncs = null;
        if (_events.ContainsKey(type) == false)
        {
            _events.Add(type, new Dictionary<double, EventBack>());
        }

        addEventfuncs = _events[type];

        if (addEventfuncs.ContainsKey(id) == false)
        {
            addEventfuncs.Add(id, func);
        }
        else
        {
            addEventfuncs[id] = func;
        }
        _events[type] = addEventfuncs;
        addEventfuncs = null;
    }

    private static void Add2DelayAddList(string type,double id,EventBack func)
    {
        lock(lockobj)
        {
            tempEventAddData = GetEventAddData();
            tempEventAddData.type = type;
            tempEventAddData.id = id;
            tempEventAddData.func = func;
            if (delayAddList == null)
            {
                delayAddList = new List<EventAddData>();
            }
            delayAddList.Add(tempEventAddData);
        }

    }

    private static List<EventAddData> eventAddDataPool;
    private static EventAddData tempEventAddData;

    private static EventAddData GetEventAddData()
    {
        if(eventAddDataPool!=null&&eventAddDataPool.Count>0)
        {
            tempEventAddData = eventAddDataPool[0];
            eventAddDataPool.RemoveAt(0);
            return tempEventAddData;
        }
        else
        {
            return new EventAddData();
        }
    }

    private static void RecoverEventAddData(EventAddData data)
    {
        if(eventAddDataPool==null)
        {
            eventAddDataPool = new List<EventAddData>();
        }
        data.Clear();
        eventAddDataPool.Add(data);
    }
    //private static List<int> inDelayAddList = new List<int>();
    /// <summary>
    /// DispatchEvent
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arg"></param>
    public static void DispatchEvent(string type,params object[] arg)
    {
        
        DispatchEventFun(type, arg);
    }
    //static List<int> inDelayAddList = new List<int>();
    private static void DispatchEventFun(string type,params object[] arg)
    {
        Dictionary<double, EventBack> funcs = null;
        //inDelayAddList.Clear(); //todo:内存优化
        List<int> inDelayAddList = new List<int>();
        lock (lockobj)
        {
            if (delayAddList != null && delayAddList.Count > 0)
            {
                for (int i = 0; i < delayAddList.Count; i++)
                {
                    if (delayAddList[i].type == type)
                    {
                        inDelayAddList.Add(i);
                    }
                }
            }
        }

        if (_events.ContainsKey(type) == false)
        {

            if (inDelayAddList.Count == 0)
            {
                return;
            }

        }
        isDispatching = true;
        dispatchCount++;
        if (dispatchCount > 50)
        {
            sw.util.LoggerHelper.Error("----------dispatchCount>50-----------:" + type);
            dispatchCount--;
            return;
        }
        if (_events.ContainsKey(type))
        {
            funcs = _events[type];
            if (funcs != null && funcs.Count > 0)
            {
                try
                {
                    foreach (KeyValuePair<double, EventBack> item in funcs)
                    {
                        if (delayRemoveList.ContainsKey(item.Key) == true)
                        {
                            continue;
                        }
                        EventBack func = item.Value;
                        if (func != null)
                        {
                            try
                            {
                                //if (type != "lua_LateUpdate" && type != "lua_Update" && type != "lua_FixedUpdate" && type != "UpdateSceneNamePosByLate")
                                //LoggerHelper.Debug("AddEventFun call  EventBack" + item.Key + ",type:" + type+","+Time.frameCount);

                                if (arg != null && arg.Length == 0)
                                {
                                    func();
                                }
                                else if (arg == null)
                                    func( );
                                else
                                {
                                    func(arg);
                                }

                            }
                            catch (System.Exception ex)
                            {
                                LoggerHelper.Error("DispatchEvent error:" + ex);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {

                }
            }
        }

        if (delayAddList != null && delayAddList.Count > 0 && inDelayAddList.Count > 0)
        {
            for (int i = 0; i < inDelayAddList.Count; i++)
            {
                if (delayAddList.Count > inDelayAddList[i])
                {
                    try
                    {
                        //LoggerHelper.Debug("AddEventFun call  delayadd " + delayAddList[inDelayAddList[i]].type  );

                        if (arg == null)
                        {
                            delayAddList[inDelayAddList[i]].func();
                        }
                        else
                        {
                            delayAddList[inDelayAddList[i]].func(arg);
                        }

                    }
                    catch (System.Exception ex)
                    {

                    }

                }
            }
        }




        dispatchCount--;
        if (dispatchCount < 0)
        {
            dispatchCount = 0;
        }
        if (dispatchCount == 0)
        {
            isDispatching = false;
            lock (lockobj)
            {
                if (delayAddList != null && delayAddList.Count > 0)
                {
                    for (int i = 0; i < delayAddList.Count; i++)
                    {
                        tempEventAddData = delayAddList[i];
                        AddEventFun(tempEventAddData.type, tempEventAddData.id, tempEventAddData.func);
                        RecoverEventAddData(tempEventAddData);
                    }
                    delayAddList = new List<EventAddData>();
                }

                if (delayRemoveList.Count > 0)
                {
                    foreach (KeyValuePair<double, string> item in delayRemoveList)
                    {
                        RemoveEventListener(item.Value, item.Key);
                    }
                    delayRemoveList = new Dictionary<double, string>();
                }
            }

        }
    }

    /// <summary>
    /// 在主线程抛出事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arg"></param>
    public static void DispatchEventMainThread(string type, params object[] arg)
    {
        EventMainThreadData data = new EventMainThreadData();
        data.type = type;
        data.param = arg;
        lock (lockobj)
        {
            _mainThreadData.Add(data);
        }
       
    }

    /// <summary>
    /// 是否侦听了
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public static bool HasEventListener(string type,double index)
    {
        if (_events.ContainsKey(type) == false)
        {
            return false;
        }
        return _events[type].ContainsKey(index);
    }

    /// <summary>
    /// 移除侦听
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public static void RemoveEventListener(string type, double index)
    {
      

        if(isDispatching)
        {
            Add2DelayDelList(index, type);
            return;
        }
        Dictionary<double, EventBack> funcs = null;
        if (_events.ContainsKey(type) == false)
        {
            return;
        }
        funcs = _events[type];
        if(funcs.ContainsKey(index))
        {
            funcs.Remove(index);
        }
    }

    private static void Add2DelayDelList(double index,string type)
    {
        lock(lockobj)
        {
            if(delayAddList!=null&&delayAddList.Count>0)
            {
                for(int i = delayAddList.Count-1;i>=0;i--)
                {
                    if(delayAddList[i].id==index)
                    {
                        delayAddList.RemoveAt(i);
                    }
                }
            }
            if (delayRemoveList.ContainsKey(index) == false)
            {
                delayRemoveList.Add(index, type);
            } 
        }
  
    }

    public static void Dispose()
    {
        _events = new Dictionary<string, Dictionary<double, EventBack>>();
        //_mainThreadTypes.Clear();
        //__mainThreadArgs.Clear();
        lock (lockobj)
        {
            _mainThreadData.Clear();
        }
        delayDispatchList.Clear();
        delayRemoveList.Clear();
        delayAddList.Clear();
        _currentIndex = 0;
    }
    public static string Dump()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("dump event:\n");
        foreach(var p in _events)
        {
            if (p.Value.Count>5)
            sb.Append(p.Key).Append(":").Append(p.Value.Count).Append("\n");
        }
        return sb.ToString();
    }
}

public class EventMainThreadData
{
    public string type;
    public object[] param;
}

public class EventAddData
{
    public string type;
    public double id;
    public EventDispatcher.EventBack func;

    public void Clear()
    {
        type = "";
        id = -1;
        func = null;
    }

}

public class DispatchEventData
{
    public string type;
    public object[] param;
}




