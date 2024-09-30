using sw.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sw.game
{
    public class GameConfig
    {
       // public static string origDir = FileUtilNew.GetStreamingPath();
        
        public const string assetBase = "";
        public const string modelBase = "/res/model";
        public const string mapBase = "/res/map";

        public static string resBaseUrl, appBaseUrl, srvListUrl, dataBaseUrl, upbyteUrl, downbyteUrl, dbBaseUrl, baseUrl, mineUrl, selectzoneUrl, getzonesUrl, noticUrl, testCdnUrl, testPhpUrl, phpUrl, commUrl, serverUrl;
        public static string[] baseUrlArr;

        public static int resVersion = 0;
        public static bool LuaEncode = false;
        public static string appVer = string.Empty, resVer = string.Empty, uiVer = string.Empty;
        //public const string configBase = "http://192.168.1.11:88";
        //public const string configBase = "http://s1370.app100715380.qqopenapp.com:8010";
    }
}
