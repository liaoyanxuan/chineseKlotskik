#if UNITYAD
using UnityEngine;
using UnityEngine.Advertisements;
#endif
public class UnityRewardedAdsScript : BaseRewardAdScript
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

    string myPlacementId = "rewardedVideo";
    bool testMode = false;


    public static UnityRewardedAdsScript instance;

    //单例模式
    public static UnityRewardedAdsScript Instance
    {
        get
        {
            if (null != instance)
            {
                return instance;
            }
            UnityRewardedAdsScript[] _monoComponents = FindObjectsOfType(typeof(UnityRewardedAdsScript)) as UnityRewardedAdsScript[];
            instance = _monoComponents[0];
            return instance;
        }

    }

    void Awake()
    {
        //初始化
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    // Initialize the Ads listener and service:
    override protected  void OnStart()
    {
        
    }


    override protected  void OnUpdate()
    {
        if (loadState == LoadState.LOADING)  //在加载状态（请求的加载中状态）
        {
            if (Advertisement.IsReady(myPlacementId))  //加载完成，准备好了
            {
                MonoBehaviour.print("UnityRewardedAdsScript-OnUpdate Ready");
                //加载完成回调
                HandleRewardedAdLoaded("testAD-from-UnityAD OnUpdate，Unity-IsReady");
            }
        }
    }


    //判断广告是否加载完毕，是否可以播放  
    override public bool isAdReady()
    {
        return Advertisement.IsReady(myPlacementId);
    }

    override protected void showRewardAd()
    {
        MonoBehaviour.print("testAD-from-UnityAD 展示广告，UnityShowRewardAd");
        base.showRewardAd();
        Advertisement.Show(myPlacementId);
    }



    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == myPlacementId)
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                // Reward the user for watching the ad to completion.
                HandleUserEarnedReward("testAD-from-UnityAD-Finished:placementId " + placementId);
                HandleRewardedAdClosed("testAD-from-UnityAD-Close:placementId " + placementId);//close才会触发队列下一个预加载

            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
                HandleRewardedAdFailedToShow("testAD-from-UnityAD-ShowResult.Skipped:placementId " + placementId);
            }
            else if (showResult == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error.");
                HandleRewardedAdFailedToShow("testAD-from-UnityAD-ShowResult.Failed:placementId " + placementId);
            }
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId)
        {
            
        }
    }

    public void OnUnityAdsDidError(string message)
    {
            // Log the error.
            HandleRewardedAdFailedToLoad("testAD-from-UnityAD:" + message);
        
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if (placementId == myPlacementId)
        {
            // Optional actions to take when the end-users triggers an ad.
            HandleRewardedAdOpening("testAD-from-UnityAD-DidStart:placementId " + placementId);
        }
    }
#endif
}
