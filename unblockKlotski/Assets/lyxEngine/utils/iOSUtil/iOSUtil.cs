using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using sw.util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using VoxelBusters.Utility;
public class iOSUtil : MonoBehaviour
{
    //1489801991
	private static string url = "https://itunes.apple.com/app/id1489801991";

	private static string imagePath;
	private static string imageDataString;

//#if UNITY_IOS

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
        imagePath = Application.persistentDataPath + "/ScreensShot.png";

#if UNITY_ANDROID
        url = "https://itunes.apple.com/app/id1489801991";
#endif
    }


    static IEnumerator ShareScreenShot()
    {
        yield return new WaitForSeconds(1f);
        ScreenCapture.CaptureScreenshot("ScreensShot.png");
        yield return new WaitForSeconds(1f);
        // NativeShare.Share("测试", Application.persistentDataPath + "/ScreenShot.png", "https://www.baidu.com");
        // showSocialSharing("测试",Application.persistentDataPath+"/ScreenShot.png");
        // (char* body, char* url, char* imageDataString, char* subject)
        using (WWW www = new WWW(imagePath))
        {
            yield return www;
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(www.bytes);
            imageDataString = Convert.ToBase64String(texture.EncodeToPNG());
        }
    }


    private static IEnumerator m_takeScreenShotCoroutine;
    private static IEnumerator m_takeCaptureTextCoroutine;
	

	static void  AttachImage(Texture2D _texture) 
    {
        if (_texture != null)
            //imageDataString = TextureExtensions.Serialise(_texture, TextureExtensions.EncodeTo.JPG);
			imageDataString = Convert.ToBase64String(_texture.EncodeToJPG());
        else
            imageDataString = null;
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Take A ScreenShot")]
#endif
    public static void ShareTest()
    {
		#if UNITY_EDITOR
			initSetting.Instance.StartCoroutine(captureScreenToDisk());
#elif UNITY_IOS || UNITY_ANDROID
			initSetting.Instance.StartCoroutine(captureScreenToShare());
#endif
    }


    private static IEnumerator captureScreenToShare()
	{
		Texture2D topScaleTexture=null;
		float scaleFactor = 1f;
		m_takeScreenShotCoroutine = TextureExtensions.TakeScreenshot((_texture) =>
			{

				// Share image,缩放
				topScaleTexture = _texture;
				// Share image,缩放,压缩
				// scaleTexture.Compress(true);

			});

		m_takeCaptureTextCoroutine = TextCaptureCameraScaler.Instance.captureTextCoroutine((_texture) => 
			{
				if (null != topScaleTexture && null != _texture)
				{
                    //4399平台特殊处理，不允许有二维码
                    if (string.Equals(AndroidUtil.getPlatformName, AndroidUtil.P4399))
                    {
                        AttachImage(topScaleTexture);
                    }
                    else
                    {
                        float deltaHeight = topScaleTexture.height - _texture.height;
                        float topHeight = topScaleTexture.height;
                        scaleFactor = deltaHeight / topHeight;
                        //上方图片缩放
                        topScaleTexture = TextureExtensions.Scale(topScaleTexture, scaleFactor);

                        Texture2D resultTexture = mergeTexture(topScaleTexture, _texture);

                        AttachImage(resultTexture);
                    }

#if !UNITY_EDITOR
    #if UNITY_ANDROID
            AndroidUtil.socialSharing(Language.getInstance().GetValue(Language.shareToSocailText), url, imageDataString, "ShareOrSave");
    #elif UNITY_IOS
//           _socialSharing(Language.getInstance().GetValue(Language.shareToSocailText),url,imageDataString,"subject");
    #endif
#endif

                }

            });

		yield return initSetting.Instance.StartCoroutine(m_takeScreenShotCoroutine);
		initSetting.Instance.StartCoroutine(m_takeCaptureTextCoroutine);
	}

    private static IEnumerator captureScreenToDisk()
	{
        Texture2D topScaleTexture=null;
        float scaleFactor = 1f;
		m_takeScreenShotCoroutine = TextureExtensions.TakeScreenshot((_texture) =>
			{
                // Share image,缩放
                topScaleTexture =_texture;
                // Share image,缩放,压缩
               // scaleTexture.Compress(true);
			});

        m_takeCaptureTextCoroutine = TextCaptureCameraScaler.Instance.captureTextCoroutine((_texture) => 
            {
                if (null != topScaleTexture && null != _texture)
                {
					float deltaHeight=topScaleTexture.height-_texture.height;
					float topHeight=topScaleTexture.height;
					//scaleFactor=deltaHeight/topHeight;
                    scaleFactor=1;
					//上方图片缩放
					topScaleTexture = TextureExtensions.Scale(topScaleTexture, scaleFactor);

                    //  Texture2D resultTexture = mergeTexture(topScaleTexture, _texture);
                    Texture2D resultTexture = topScaleTexture;
                    byte[] bytes = resultTexture.EncodeToJPG();// resultTexture.EncodeToJPG();
                    string file = Application.persistentDataPath + "/screenShot" + scaleFactor + ".jpg";
                    DebugEx.Log("Take A ScreenShot:"+file);
                    System.IO.File.WriteAllBytes(file, bytes);
                }

            });

		yield return initSetting.Instance.StartCoroutine(m_takeScreenShotCoroutine);
        initSetting.Instance.StartCoroutine(m_takeCaptureTextCoroutine);
	}

    private static Texture2D mergeTexture(Texture2D textureTop,Texture2D textureBottom)
    {
		Texture2D mergeTexture = new Texture2D(textureBottom.width, textureTop.height + textureBottom.height);
        Texture2D [] texts=new Texture2D[2];
        texts[0]=textureTop;
        texts[1]=textureBottom;

		int topStartX = (int)Mathf.Floor((textureBottom.width - textureTop.width) / 2);

		mergeTexture.SetPixels(topStartX, textureBottom.height, textureTop.width, textureTop.height, textureTop.GetPixels());

        mergeTexture.SetPixels(0, 0, textureBottom.width, textureBottom.height, textureBottom.GetPixels());

        mergeTexture.Apply();
     
        return mergeTexture;
    }


    public static string getCurIOSLang()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
            return "zh-Hans";
#elif UNITY_IOS
        	return _curiOSLang();
#endif
    }

    public static string specicalDeviceUUID() 
    {
        //0.正常模式； 1.真实广告测试；
        string specicalDeviceUUIDstr = "";
#if !UNITY_EDITOR
#if UNITY_ANDROID
                specicalDeviceUUIDstr = AndroidUtil.getSpecicalDeviceUUID;
#elif UNITY_IOS
                specicalDeviceUUIDstr = _specicalDeviceUUID();
#endif
#endif
        if (false==string.IsNullOrEmpty(specicalDeviceUUIDstr))
        {
            return specicalDeviceUUIDstr;
        }
        return string.Empty;

    }


    public static string umpDeviceIdentifiers()
    {
        //0.正常模式； 1.真实广告测试；
        string umpDeviceIdentifiersstr = "";
#if !UNITY_EDITOR
#if UNITY_ANDROID
                umpDeviceIdentifiersstr = AndroidUtil.getUmpDeviceIdentifiersstr;
#elif UNITY_IOS
                umpDeviceIdentifiersstr = _umpDeviceIdentifier();
#endif
#endif
        if (false == string.IsNullOrEmpty(umpDeviceIdentifiersstr))
        {
            return umpDeviceIdentifiersstr;
        }
        return string.Empty;

    }

    public static bool isRealAd() 
    {
        int realAdInt = 1;
#if !UNITY_EDITOR
    #if UNITY_ANDROID
              realAdInt = AndroidUtil.isRealAd;
    #elif UNITY_IOS
              realAdInt =_isRealAd();
    #endif
#endif

        if (0==realAdInt)
            {
                    return false;
            }else
            {
                return true;
            }

    }


    public static void copyTextToClipboard(string targetString)
    {
#if !UNITY_EDITOR && UNITY_IOS
             _copyTextToClipboard(targetString);
#endif    

    }

    public static Boolean isTCRewardAdReady()
    {
#if !UNITY_EDITOR && UNITY_IOS
           int result=_isTCRewardAdReady();
        if (1 == result)
        {
            return true;
        }
        return false;
#else

        return false;
#endif
    }

    public static void createAndLoadTCRewardVideoAd()
    {
#if !UNITY_EDITOR && UNITY_IOS
             _createAndLoadTCRewardVideoAd();
#endif

    }

    public static void showTCRewardVideoAd()
    {
#if !UNITY_EDITOR && UNITY_IOS
        _showTCRewardVideoAd();
#endif

    }


    public static void inAppEvaluate()
    {
#if !UNITY_EDITOR && UNITY_IOS
        _inAppEvaluate();
#endif

    }


    public static void requestIDFA(Action<string> callback)
    {
#if !UNITY_EDITOR && UNITY_IOS
        _requestIDFA();
#else
        callback("1");
#endif
    }

    public static void goToProductPage(string appid)
    {
#if !UNITY_EDITOR && UNITY_IOS
        _goToProductPage(appid);
#endif
    }


    public static void setStatusBarLight(int val)
    {

#if !UNITY_EDITOR && UNITY_IOS
          _setStatusBarLight(val);
#endif
    }

    public static void setBannerHeight(int val)
    {

#if !UNITY_EDITOR && UNITY_IOS
          _setBannerHeight(val);
#endif
    }



    public static int getScaleFactor()
    {

#if !UNITY_EDITOR && UNITY_IOS
          return _getScaleFactor();
#endif
        return 0;
    }

    public static bool isChineseCNY()
    {

        int _isCNY = 0;


        //安卓且穿山甲聚合
        #if CSJADJH && UNITY_ANDROID
             _isCNY = 1;
        #endif

    #if UNITY_EDITOR
            if (TestManager.Instance.isChianCNY)
            {
                _isCNY = 1;
            }
    #else
        #if  UNITY_IOS
              _isCNY=_isChineseCNY();
        #endif
    #endif

        if (_isCNY == 1)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }


    public static bool isNeedGDPR()
    {
        int _needGDPR = 0;

#if UNITY_EDITOR
            if (TestManager.Instance.isGDPR)
            {
                _needGDPR = 1;
            }
#else
#if UNITY_IOS
            _needGDPR=_isNeedGDPR();
#endif

#if GOOGLEPLAY
            _needGDPR=1;
#endif
#endif


        if (_needGDPR == 1)
        {
            return true;
        }
        return false;
    }


    public static int IsDebugTest
    {
        get
        {

#if !UNITY_EDITOR && UNITY_IOS
              return _IsDebugTest();
#endif

            return 0;
        }
    }

    public static int IsFPSDebugTest
    {
        get
        {

#if !UNITY_EDITOR && UNITY_IOS
              return _IsFPSDebugTest();
#endif

            return 0;
        }
    }

    public static void loadOpenAd()
    {

#if !UNITY_EDITOR && UNITY_IOS
          _loadOpenAd();
#endif
    }

    public static void presentOpenAd()
    {

#if !UNITY_EDITOR && UNITY_IOS
          _presentOpenAd();
#endif
    }

    public static int IsDebugLayoutTest
    {
        get
        {


#if !UNITY_EDITOR && UNITY_IOS
              return _IsDebugLayoutTest();
#endif

            return 0;
        }
    }


#if UNITY_IOS
    [DllImport("__Internal")]
        private static extern void _socialSharing(string body, string url, string imageDataString, string subject);

        [DllImport("__Internal")]
        private static extern void _shareWeb(string body, string url, string imagePath, string subject);


        [DllImport("__Internal")]
        private static extern void _inAppEvaluate();

         [DllImport("__Internal")]
        private static extern void _requestIDFA();

        [DllImport("__Internal")]
        private static extern void _goToProductPage(string appid);

        // ios手机的当前语言 "en"、“zh"、“zh-Hans"、"zh-Hant"  
        [DllImport("__Internal")]
        private static extern string _curiOSLang();

        [DllImport("__Internal")]
        private static extern string _specicalDeviceUUID();


        [DllImport("__Internal")]
        private static extern string _umpDeviceIdentifier();

        [DllImport("__Internal")]
        private static extern int _isRealAd();

        [DllImport("__Internal")]
        private static extern void _copyTextToClipboard(string targetString);

        [DllImport("__Internal")]
        private static extern int _isTCRewardAdReady();

        [DllImport("__Internal")]
        private static extern void _createAndLoadTCRewardVideoAd();

        [DllImport("__Internal")]
        private static extern void _showTCRewardVideoAd();

        [DllImport("__Internal")]
        private static extern void _setStatusBarLight(int val);
    
        [DllImport("__Internal")]
        private static extern void _setBannerHeight(int val);

    [DllImport("__Internal")]
        private static extern int _getScaleFactor();

        [DllImport("__Internal")]
        private static extern int _isChineseCNY();

    [DllImport("__Internal")]
    private static extern int _isNeedGDPR();

    [DllImport("__Internal")]
    private static extern int _loadOpenAd();


    [DllImport("__Internal")]
    private static extern int _presentOpenAd();

    [DllImport("__Internal")]
        private static extern int _IsDebugTest();

        [DllImport("__Internal")]
        private static extern int _IsDebugLayoutTest();

        [DllImport("__Internal")]
        private static extern int _IsFPSDebugTest();

#endif



}

