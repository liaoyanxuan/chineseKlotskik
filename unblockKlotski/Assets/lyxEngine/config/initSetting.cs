using liaoyanxuan.common.injector;
using sw;
using sw.game.evt;
using sw.util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initSetting : MonoBehaviour {
    //使用的旧金山字体SanFrancisco，微信：DIN的字体基础上微调
    public Font SanFranciscoFont;

    private static initSetting instance;


    //单例模式
    public static initSetting Instance
    { 
        get
        {
			if (null != instance) 
			{
				return instance;
			}
            initSetting[]  _monoComponents=FindObjectsOfType(typeof(initSetting)) as initSetting[];
            instance = _monoComponents[0];
            return  instance;
        }
       
    }

	// Use this for initialization
	void Start () 
    {
        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        AndroidDelCache.DeleteLogs();
        
    }
	


    //当程序获得或失去焦点；
    void OnApplicationFocus(bool isFocus)
    {
       
        if (isFocus)
        {
            Application.targetFrameRate = 60;
       
            QualitySettings.vSyncCount = 1;

            DebugEx.LogImportant("Application.targetFrameRate", Application.targetFrameRate);
            DebugEx.LogImportant("Screen.sleepTimeout", Screen.sleepTimeout);

        }
       
    }


    //退出应用（安卓和iOS不一定会调用！！！）
    void OnApplicationQuit()
    {
        AndroidDelCache.DeleteLogs();
        
    }
}
