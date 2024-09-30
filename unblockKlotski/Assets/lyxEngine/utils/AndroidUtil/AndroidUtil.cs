using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace sw.util
{
    public class AndroidUtil
    {

        public static string P4399 = "4399";
#if UNITY_ANDROID
        static AndroidJavaObject obj_Activity;

        public static AndroidJavaObject getActivity()
        {
            if(obj_Activity == null)
            {
                AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
            }
            return obj_Activity;
        }

        public static void ShowBannerView(AndroidJavaObject view)
        {
            AndroidJavaObject adViewManager = new AndroidJavaClass("com.qq.e.union.demo.UnionAdViewManager").CallStatic<AndroidJavaObject>("getInstance");
            adViewManager.Call("showBannerAdView", getActivity(), view);
        }

        public static AndroidJavaObject NewAdContainer()
        {
            AndroidJavaObject adViewManager = new AndroidJavaClass("com.qq.e.union.demo.UnionAdViewManager").CallStatic<AndroidJavaObject>("getInstance");
            return adViewManager.Call<AndroidJavaObject>("newFrameLayout", getActivity());
        }
#endif
        public static void ReportMsg(string msg)
        {

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)              
                    getActivity().Call("ReportError", msg);

#endif
     

             
        }
		public static float getScreenBrightness()
		{
			
#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<float>("getScreenBrightness");
			else
#endif
			
			return 0f;			
		}
        public static void setScreenBrightness(float val)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("PSMScreenBrightness", val);
#endif
        }
        public static void PSMDelayTime(float tm)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("PSMDelayTime", tm);
#endif
        }
        public static float PSMGetDelayTime()
        {
            float tm = 0f;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                tm = getActivity().Call<float>("PSWGetDelayTime");
#endif
            return tm;
        }
		public static void PSMStart()
		{
#if UNITY_ANDROID
			if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("PSMStart");
#endif
		}
        public static void PSMEnd()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("PSMEnd");
#endif
        }
        public static long getUnixTimeByCalendar()
        {
            long ticks = 0;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                ticks = getActivity().Call<long>("getUnixTimeByCalendar");
            }
#endif
            return ticks;
        }
        public static float getElectricValue()
        {
            float val = 0f;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                val = getActivity().Call<float>("getElectricValue");
            }
#endif
            return val;
        }
        public static string getPackageName()
        {
            DebugEx.Log("AndroidUtil getPackageName");
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<string>("getPackageName");
            else
#endif
                return "";
        }


        public static string getExternalFilesDir()
        {
            DebugEx.Log("AndroidUtil getExternalFilesDir");
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<string>("getExternalFilesDir");
            else
#endif
                return Application.persistentDataPath;
        }

        public static string GetDownLoadPath()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<string>("GetDownloadPath");
            else
#endif
            return "";
        }
        //复制
        public static void CopyLabelText(string msg)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("CopyLabelText", msg, getActivity());
#endif
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void ShowExit()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("ShowExit");
#endif
        }



        public static void requireVerifyStoragePermissions()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("requireVerifyStoragePermissions");
#endif
        }


        /// <summary>
        /// 获取内存信息
        /// </summary>
        /// <returns></returns>
        public static string GetMemInfo()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<string>("GetMemInfo");
            else
#endif
                return "";

        }
        public static int GetUSS()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<int>("GetUss");
            else
#endif
                return 0;

        }

        public static void TestCrash()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                 getActivity().Call("TestCrash");
#endif
        }
        /// <summary>
        /// 获取版本版本号
        /// </summary>
        /// <returns></returns>
        public static string GetVersionName()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<string>("GetVersionName");
            else
#endif
                return "";
        }
        public static int GetVersionCode()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
           
                return getActivity().Call<int>("GetVersionCode");
           
#endif
                return 52;
        }
        public static void Restart()
        {
           
#if UNITY_ANDROID && !UNITY_EDITOR
             getActivity().Call("Restart");
#endif
        }
        static string _fileDir;
        public static string getFileDir()
        {
            
#if UNITY_ANDROID && !UNITY_EDITOR
            if(string.IsNullOrEmpty(_fileDir))
            {
                _fileDir = getActivity().Call<string>("getFileDir");
            }
            return _fileDir;    
                
           
#endif
            return "";
        }
        public static string GetDCIMPath()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                return getActivity().Call<string>("GetDCIMPath");
            else
#endif
                return "null";
        }
        public static void RefreshFile(string filename)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                getActivity().Call("RefreshFile", filename);
#endif
        }

        public static int batteryLevel
        {
           

            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    return getActivity().Call<int>("batterylevel");
#endif
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
         
        }

        public static int isbatteryCharge
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    return getActivity().Call<int>("batteryChargeing");
#endif
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public static int isWifiNet //是否是WIFI网络
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    return getActivity().Call<int>("wifiBo");
#endif
                    return 1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public static int wifiLevel //WIFI信号 1-5
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
 #if UNITY_ANDROID
                    return getActivity().Call<int>("wifiRssi");
#endif
                    return 1;
                }
                else
                {
                    return 1;
                }

            }
        }

        public static int netWorkType //网络格式 数字+G
        {
            get
            {
               
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    return getActivity().Call<int>("getNetworkClass");
#endif
                    return 2;
                }
                else
                {
                    return 2;
                }

            }
        }
        // imei
        public static string imeiStr //imei码。双卡有两个，返回第一个
        {
            get
            {

                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
               
                  return SystemInfo.deviceUniqueIdentifier;
                   
#endif
                    return "";
                }
                else
                {
                    return "";
                }

            }
        }

        public static byte[] GetStreamAssetByte(string path)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    return getActivity().Call<byte[]>("getFromAssetss", path);
                }
                catch(Exception ex)
                {
                    DebugEx.LogError(ex.Message);
                    return null;
                }
            }
            else
            {
                return LoadByteByFile(StringTools.AppendString(Application.streamingAssetsPath, "/", path));
            }
#else
            
            return LoadByteByFile(StringTools.AppendString(Application.streamingAssetsPath, "/", path));
#endif

        }

        public static byte[] LoadByteByFile(string path)
        {
            if (Tools.FileManager.IsFileExists(path) == true)
            {
                return Tools.FileManager.ReadFileBytes(path);
            }
            else
            {
                return null;
            }
        }
#if UNITY_ANDROID && !UNITY_EDITOR
	private static AndroidJavaClass	jc = null;
	private static AndroidJavaObject jo = null;
    private static int _channelId = -1;
    private static string _qqAid = "";
#endif
        public static int GetChannelID()
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
            if(_channelId>=0)
            {
                return _channelId;
            }
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            string channelIDStr = jo.Call<string>("GetMetaDataValue", "game_php_channel_id");
            _channelId = int.Parse(channelIDStr);
            return _channelId;
#else
            return 1;
#endif
        }
        public static string GetCpuInfo()
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
           
                return getActivity().Call<string>("GetCpuInfo");
#else
                return "";
#endif
        }

       
        public static string GetQQAid()
        {
#if UNITY_ANDROID&&!UNITY_EDITOR
            if(string.IsNullOrEmpty(_qqAid)==false)
            {
                return _qqAid;
            }
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            string tempStr = jo.Call<string>("GetMetaDataValue", "game_qq_aid");
            if (string.IsNullOrEmpty(tempStr)==false)
            {
                _qqAid = tempStr;
            }
            return _qqAid;
#else
            return "";
#endif
        }
        private static AndroidJavaClass _helper;
        private static AndroidJavaClass AndroidHelper
        {
            get
            {
                if (_helper != null) return _helper;

                _helper = new AndroidJavaClass("com.github.KEngine.AndroidHelper");

                if (_helper == null)
                    throw new Exception("[KEngineAndroidPlugin.cs]Error on Android Plugin. Check if KEngine.Android.jar file exist in your Plugins/Android/libs? KEngine DLL mode also not support.");

                return _helper;
            }
        }


        public static byte[] ReadAssetFile(string filename)
        {
            //DebugEx.Log("read assset file:" + filename);
            AndroidJNI.PushLocalFrame(0);
            byte[] re = null;
#if UNITY_ANDROID
            try
            {

                //re = AndroidHelper.CallStatic<byte[]>("getAssetBytes", filename);
                //DebugEx.LogError("ReadAssetFile000000:"+ filename);
                re = getActivity().CallStatic<byte[]>("getAssetBytes", filename);
            }
            catch(Exception ex)
            {
                DebugEx.LogError("ReadAssetFile " + filename + ",error:" + ex);
                
            }
            
            AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
#endif
            return re;
        }


        public static string getSpecicalDeviceUUID
        {
            get
            {
                
                #if UNITY_ANDROID && !UNITY_EDITOR

                    return getActivity().Call<string>("GetSpecicalDeviceUUID");
                #endif

                
                return string.Empty;
            }
        }

        public static string getUmpDeviceIdentifiersstr
        {
            get
            {

#if UNITY_ANDROID && !UNITY_EDITOR

                    return getActivity().Call<string>("GetUmpDeviceIdentifiersstr");
#endif


                return string.Empty;
            }
        }


        


        public static string getPlatformName
        {
            get
            {

#if UNITY_ANDROID && !UNITY_EDITOR

                    return getActivity().Call<string>("GetPlatformName");
#endif
               
                return string.Empty;
            }
        }


        public static int isRealAd
        {
            get
            {
                
                #if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("IsRealAd");
                #endif
                return 1;
            }
        }


        public static int IsDebugTest
        {
            get
            {

                #if BYTEDGAME
                                return 0;
                #endif

                #if UNITY_EDITOR
                        if (TestManager.Instance.isDebugTest)
                        { 
                            return 1;
                        }
                #else
                        #if UNITY_ANDROID
                            return getActivity().Call<int>("IsDebugTest");
                        #endif
                #endif
                        return 0;
            }
        }

        public static int IsDebugLayoutTest
        {
            get
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("IsDebugLayoutTest");
#endif


#if UNITY_IOS
              return  iOSUtil.IsDebugLayoutTest;
#endif


                return 0;
            }
        }

        public static int IsFPSDebugTest
        {
            get
            {
#if DEBUG
                return 0;
#endif

#if BYTEDGAME
                return 0;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("IsFPSDebugTest");
#endif
#if UNITY_EDITOR
                return 1;
#endif
                return 0;
            }
        }



        public static void initADs()
        {

#if BYTEDGAME
            return;
#endif

#if GOOGLEPLAY
            return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                     getActivity().Call("initADs");
#endif
        }

        public static int getPlatformId
        {
            get
            {

#if BYTEDGAME
                return 1;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("getPlatformId");
#endif
#if UNITY_IOS || GOOGLEPLAY
                return 1;
#endif

                return 2;
            }
        }

        public static int iSShowPrivacyPolicy
        {
            get
            {

#if BYTEDGAME
                return 0;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("iSShowPrivacyPolicy");
#endif
#if UNITY_IOS || GOOGLEPLAY
                return 0;
#endif

                return 0;
            }
        }

        public static int iSShowIndependentPrivacyPolicy
        {
            get
            {

#if BYTEDGAME
                return 0;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("iSShowIndependentPrivacyPolicy");
#endif
#if UNITY_IOS || GOOGLEPLAY
                return 0;
#endif

                return 1;
            }
        }

        public static void showIndependentPrivacy()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                     getActivity().Call("showIndependentPrivacy");
#endif
        }

        public static int iSShowTaptapPrivacyPolicy
        {
            get
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("iSShowTaptapPrivacyPolicy");
#endif

                return 0;
            }
        }


        public static int iSAgeTip
        {
            get
            {
#if BYTEDGAME
                return 1;
#endif

#if GOOGLEPLAY
                return 0;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                         return getActivity().Call<int>("iSAgeTip");
#endif

#if UNITY_IOS
                         return 0;
#endif

                return 1;
            }
        }


        public static int isNeedNameCodeVerifed
        {
            get
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("isNeedNameCodeVerifed");
#endif

                return 0;
            }
        }

        public static int iSTaptapAntiAddiction
        {
            get
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("iSTaptapAntiAddiction");
#endif

#if UNITY_EDITOR
                return 1;
#endif

                return 0;
            }
        }


        public static bool isCanShareOther()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
              return getActivity().Call<bool>("isCanShareOther");
#endif
            return true;
        }

        public static void socialSharing(string body, string url, string imageDataString, string subject)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                     getActivity().Call("SocialSharing", body, url, imageDataString, subject);
#endif


        }

        public static int copyTextToClipboard(string str2paste)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                    return getActivity().Call<int>("copyTextToClipboard", str2paste);
#endif
            return 0;

        }


        public static void createAndLoadTCRewardVideoAd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
              getActivity().Call("createAndLoadTCRewardVideoAd");
#endif
        }


        public static bool isTCRewardAdReady()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
              return getActivity().Call<bool>("isTCRewardAdReady");
#endif
            return false;
        }

        public static void showTCRewardVideoAd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
              getActivity().Call("showTCRewardVideoAd");
#endif
        }

        public static bool isGooglePlayServicesAvailable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
              return getActivity().Call<bool>("isGooglePlayServicesAvailable");
#endif
            return false;
        }


        public static void HideSplashScreen()
        {
#if BYTEDGAME
            return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
             getActivity().Call("hideSplash");
#endif
        }

		// ip
		public static string getIP
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    return getActivity().Call<string>("GetIP");
#endif
                    
                }
                
                    return "";
                
            }
        }
        public static int GetScreenBrightness()
        {
            
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    return getActivity().Call<int>("GetScreenBrightness");
#endif
                    
                }
               
                    return 1;
                
            }
       
        public static void   SetScreenBrightness(int val)
        {
            
                if (Application.platform == RuntimePlatform.Android)
                {
#if UNITY_ANDROID
                    DebugEx.Log("idletest "+val);
        //            AndroidJavaObject Activity = null;
        //Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        //Activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
        //             AndroidJavaObject Window = null, Attributes = null;
        //             Window = Activity.Call<AndroidJavaObject>("getWindow");
        //             Attributes = Window.Call<AndroidJavaObject>("getAttributes"); 
        //             Attributes.Set("screenBrightness", val/255f); 
        //             Window.Call("setAttributes", Attributes); 
        //             })); 


                    getActivity().Call("SetScreenBrightness", val);
#endif
                
                }
               
            }

        public static void setStatusBarLight(int val)
        {

#if !UNITY_EDITOR && UNITY_ANDROID
                getActivity().Call("setStatusBarLight", val);
#endif
        }


        /// <summary>
        ///  隐藏上方状态栏
        /// </summary>
        public static void HideStatusBar()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
        setStatusBarValue(1024); // WindowManager.LayoutParams.FLAG_FULLSCREEN; change this to 0 if unsatisfied
#endif
        }


        private static int newStatusBarValue;
        /// <summary>
        ///  显示上方状态栏
        /// </summary>
        public static void ShowStatusBar()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
        setStatusBarValue(2048); // WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN
#endif
        }

        private static void setStatusBarValue(int value)
        {
            newStatusBarValue = value;
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(setStatusBarValueInThread));
                }
            }
        }

        private static void setStatusBarValueInThread()
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                    {
                        window.Call("setFlags", newStatusBarValue, newStatusBarValue);
                    }
                }
            }
        }


    }
}
