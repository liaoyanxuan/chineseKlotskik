using UnityEngine;
using System.Collections;
namespace sw
{
    public class AppConfig
    {
        public static bool noUpdate = false;
        public static bool useLocalUrl = false;

        public static bool useInsideData = false;
        public static bool useOutsideResource = false;
        public static bool useOutsideData = false;
        public static bool useOutsideLua = false;
        public static bool useOutsideUI = false;
        public static bool encryptPath = true;
        public static bool useInsideUI = false;
        public static bool ShowFPS = false;
        public static bool isDebug = false;
        public static bool useSDK = true;
        public static bool testApp = false; 
        public static bool SDKInit = false;
        public static int testPTID = 0;
        public static bool noServiceState = false;
        public static string channelName = "";
        public static string channelId = "";
        public static string gameId = "";
        public static string subGameId = "";
        public static string productId = "";
        public static string secretKey = "";
        public static string appid = "";
        public static string appkey = "";
        public static string isMusicingMusicing = "0";
        public static int intJHSDK = 0;
        public static string localVer = "";
        public static bool debugLua = false;
        public static int initBrightness=0;
        public static bool dynamicScene = true;
        public static bool isLowMemroy = false;
        public static int testDownRate = 0;
        //销毁场景中不可见的物体，以节省内存
        public static bool destroyNoneVisSceneObj = false;
        public static bool testMap = false;
        public static string appConfigTestCdn = "";
        //public static bool autofight = true;

        public static bool EDITOR_DEBUG
        {
            get {
#if HOT_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
    }
}

