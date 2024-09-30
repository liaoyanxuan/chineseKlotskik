#if GOOGLEPLAY||UNITY_IOS&&!SIMULATOR
using GoogleMobileAds.Api;
using sw.util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAdmobID {


    private static string appId = "unexpected_platform";
    private static string rewardAdUnitId = "unexpected_platform";
    private static string bannerAdUnitId = "unexpected_platform";
    private static string interstitialAdUnitId = "unexpected_platform";
    private static string openAdUnitId = "unexpected_platform";

    static Hyperbyte.Ads.GoogleMobileAdsSettings settings;

    static GoogleAdmobID()
    {
        settings = (Hyperbyte.Ads.GoogleMobileAdsSettings) (Resources.Load("AdNetworkSettings/GoogleMobileAdsSettings"));
    }

    public static string AppId
    {
      get 
      { 
            appId = settings.GetAppId();
            return appId ;
      }
 
    }

    public static string BannerAdUnitId
    {
        get
        {
            if (iOSUtil.isRealAd())
            {
                DebugEx.Log("GoogleAdmob_BannerAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());
                bannerAdUnitId = settings.GetBannetAdUnitId();

                DebugEx.Log("bannerAdUnitId:" + bannerAdUnitId);
            }
            else
            {
                DebugEx.Log("GoogleAdmob_BannerAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());
#if UNITY_ANDROID

                bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";     //测试ID
#elif UNITY_IPHONE
                  
                bannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";     //测试ID
#endif
            }



            return bannerAdUnitId;
        }
    }

    public static string InterstitialAdUnitID
    {
       
        get
        {
            if (iOSUtil.isRealAd())
            {
                DebugEx.Log("GoogleAdmob_InterstitialAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());

                interstitialAdUnitId = settings.GetInterstitialAdUnityId();
                DebugEx.Log("interstitialAdUnitId:" + interstitialAdUnitId);
            }
            else
            {
                DebugEx.Log("GoogleAdmob_InterstitialAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());
#if UNITY_ANDROID
                      
               interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";     //测试ID
#elif UNITY_IPHONE

                interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";     //测试ID
#endif
            }
            return interstitialAdUnitId;
        }
    }

    public static string RewardAdUnitId
    {
        get
        {
            if (iOSUtil.isRealAd())
            {
                DebugEx.Log("GoogleAdmob_RewardAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());

                rewardAdUnitId = settings.GetRewardedAdUnitId();

                DebugEx.Log("rewardAdUnitId:" + rewardAdUnitId);
            }
            else
            {
                DebugEx.Log("GoogleAdmob_RewardAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());
#if UNITY_ANDROID
                      
                rewardAdUnitId = "ca-app-pub-3940256099942544/5224354917";     //测试ID
#elif UNITY_IPHONE
               
                rewardAdUnitId = "ca-app-pub-3940256099942544/1712485313";     //测试ID
#endif
            }
            return rewardAdUnitId; 
        }
       
    }


    public static string OpenAdUnitId
    {
        get
        {
            if (iOSUtil.isRealAd())
            {
                DebugEx.Log("GoogleAdmob_OpenAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());

                openAdUnitId = settings.GetOpenAdUnitId();

                DebugEx.Log("openAdUnitId:"+ openAdUnitId);
            }
            else
            {
                DebugEx.Log("GoogleAdmob_OpenAdUnitId_isRealAd():" + iOSUtil.isRealAd().ToString());
#if UNITY_ANDROID
                      
                openAdUnitId = "ca-app-pub-3940256099942544/3419835294";     //测试ID
#elif UNITY_IPHONE

                openAdUnitId = "ca-app-pub-3940256099942544/5662855259";     //测试ID
#endif
            }
            return openAdUnitId;
        }

    }



    public static AdRequest getAdRequest() 
    {
        AdRequest request = null;
        // Create an empty ad request.
        string testDevice = iOSUtil.specicalDeviceUUID();

        if (false == string.IsNullOrEmpty(testDevice))
        {
            List<string> deviceIds = new List<string>();
            deviceIds.Add(testDevice);

            RequestConfiguration requestConfiguration = new RequestConfiguration();

            requestConfiguration.TestDeviceIds= deviceIds;

            MobileAds.SetRequestConfiguration(requestConfiguration);

            DebugEx.Log("GoogleAdmob_getAdRequest_testDevice:" + testDevice);
        }

        request = new AdRequest();

        return request;
    }
        
}
#endif