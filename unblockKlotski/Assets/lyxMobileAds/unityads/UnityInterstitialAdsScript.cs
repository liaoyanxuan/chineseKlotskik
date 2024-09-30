#if UNITYAD
using UnityEngine;
using UnityEngine.Advertisements;
#endif

public class UnityInterstitialAdsScript  : BaseInterstitialAdScript
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

    string myPlacementId = "interstitial";
    bool testMode = false;



    public static UnityInterstitialAdsScript instance;

    //单例模式
    public static UnityInterstitialAdsScript Instance
    {
        get
        {
            if (null != instance)
            {
                return instance;
            }
            UnityInterstitialAdsScript[] _monoComponents = FindObjectsOfType(typeof(UnityInterstitialAdsScript)) as UnityInterstitialAdsScript[];
            instance = _monoComponents[0];
            return instance;
        }
    }

    void Start()
    {
        Advertisement.AddListener(this);
        // Initialize the Ads service:
        Advertisement.Initialize(gameId, testMode);
    }

    private void ShowInterstitialAd()
    {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady(myPlacementId))
        {
            Advertisement.Show(myPlacementId);
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    //广告是否准备好
    override public bool isAdReady()
    {
        return Advertisement.IsReady(myPlacementId);
    }

    //播放广告
    override protected void showAd()
    {
        Debug.Log("UnityInterstitial--showAd!");
        Advertisement.Show(myPlacementId);
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
        
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if (placementId == myPlacementId)
        {
            Debug.Log("UnityInterstitialAds--DidStart!");
            base.adSussOpenCallBack();
        }
    }
#endif
}
