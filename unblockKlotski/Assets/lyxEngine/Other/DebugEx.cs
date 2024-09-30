using Newtonsoft.Json;
using sw.util;
using UnityEngine;


public static class DebugEx
{
    private static double logIndex = 1;

    [System.Diagnostics.Conditional("LOG")]
    public static void Log(params object[] message)
    {
        if (iOSUtil.IsDebugTest == 1 || AndroidUtil.IsDebugTest == 1)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }
            Debug.Log(s + "\t" + "logIndex:" + logIndex++);
        }
    }


    public static void LogImportant(params object[] message)
    {
        var s = "";
        foreach (var tmpS in message)
        {
            s += tmpS + "\t";
        }
        Debug.Log(s + "\t" + "logIndex:" + logIndex++);
    }


    [System.Diagnostics.Conditional("LOG")]
    public static void LogString(string s)
    {
            
        Debug.Log(s + "\t" + "logIndex:" + logIndex++);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogObject(params object[] message)
    {
        var s = "";
        foreach (var tmpS in message)
        {
            s += JsonConvert.SerializeObject(tmpS) + "\t";
        }
        Debug.Log(s + "\t" + "logIndex:" + logIndex++);
    }
        
    [System.Diagnostics.Conditional("LOG")]
    public static void LogWarning(params object[] message) {
        var s = "";
        foreach (var tmpS in message)
        {
            s += tmpS + "\t";
        }
        Debug.LogWarning(s + "\t" + "logIndex:" + logIndex++);
    }

    [System.Diagnostics.Conditional("LOG")]
    public static void LogError(object message, Object obj = null) {
        Debug.LogError(message, obj);
    }
}

