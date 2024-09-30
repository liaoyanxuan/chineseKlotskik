using UnityEngine;
using System.Collections;
using System;
namespace sw.util
{
    public class TimeHelper
    {


        private static DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        private static DateTime now;


        static public double GetSystemTime()
        {
            now = System.DateTime.Now;
            double ts = (now - startTime).TotalMilliseconds;
            return ts;
        }
    }
}
