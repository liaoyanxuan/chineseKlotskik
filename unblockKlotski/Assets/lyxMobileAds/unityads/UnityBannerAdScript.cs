#if UNITYAD
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
#endif

public class UnityBannerAdScript : BaseBannerAdScript
#if UNITYAD
    , IUnityAdsListener
#endif
{
#if UNITYAD
#if UNITY_ANDROID
        string gameId = "3990721";
#elif UNITY_IOS
        string gameId = "3990720";
#else
        string gameId = "3990720";
#endif

    string myPlacementId = "banner";
    bool testMode = false;



    public static UnityBannerAdScript instance;

    //单例模式
    public static UnityBannerAdScript Instance
    {
        get
        {
            if (null != instance)
            {
                return instance;
            }
            UnityBannerAdScript[] _monoComponents = FindObjectsOfType(typeof(UnityBannerAdScript)) as UnityBannerAdScript[];
            instance = _monoComponents[0];
            return instance;
        }
    }


    override protected void OnStart()
    {
        base.OnStart();
       
    }


    //展示广告,子类必须调用，设置loadToshow为false，是否要求立即播放
    override public void showBanner()
    {
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(ShowBannerWhenInitialized());
    }


    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        Advertisement.Banner.Show(myPlacementId);
    }


    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == myPlacementId)
        {

        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == myPlacementId)
        {

        }
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log(this.GetType().Name+" Banner Error:"+ message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if (placementId == myPlacementId)
        {
            Debug.Log("UnityBannerAds--DidStart!");
          
        }
    }
#endif
}
