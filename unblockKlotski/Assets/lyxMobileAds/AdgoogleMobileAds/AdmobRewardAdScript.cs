#if GOOGLEPLAY || UNITY_IOS
using GoogleMobileAds.Api;
#endif

using System;
using System.Collections;
using UnityEngine;

//谷歌广告插件
public class AdmobRewardAdScript : BaseRewardAdScript
{


#if GOOGLEPLAY  || UNITY_IOS
    public static AdmobRewardAdScript instance;

    //单例模式
    public static AdmobRewardAdScript Instance
	{ 
		get
		{
			if (null != instance) 
			{
				return instance;
			}
			AdmobRewardAdScript[]  _monoComponents=FindObjectsOfType(typeof(AdmobRewardAdScript)) as AdmobRewardAdScript[];
			instance = _monoComponents[0];
			return  instance;
		}

	}

    private void Awake()
    {

        MobileAds.Initialize(initStatus => { });

    }

    override protected void OnStart()
    {

    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////
    /// 激励广告
    /// </summary>
    private RewardedAd rewardedAd;
    override public void CreateAndLoadAd(bool needErrorTip = false)
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            loadState = LoadState.NONE;
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        //#if !UNITY_EDITOR
        base.CreateAndLoadAd(needErrorTip);

        string rewardAdUnitId = GoogleAdmobID.RewardAdUnitId;


        AdRequest adRequest = GoogleAdmobID.getAdRequest();

        // Load the rewarded ad with the request.
        RewardedAd.Load(rewardAdUnitId, adRequest ,(RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load an ad " +
                               "with error : " + error);

                HandleRewardedAdFailedToLoadAdmob();
                return;
            }

            Debug.Log("Rewarded ad loaded with response : "
                      + ad.GetResponseInfo());

            rewardedAd = ad;
            RegisterEventHandlers(rewardedAd);

            HandleRewardedAdLoadedAdmob();
        });
        //#else
        //   HandleRewardedAdFailedToLoad("AdmobADNotSupportEditor");
        //#endif

    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
            HandleRewardedAdOpeningAdmob();
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            HandleRewardedAdClosedAdmob();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            HandleRewardedAdFailedToShowAdmob();
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    //判断广告是否加载完毕，是否可以播放  
    override public bool isAdReady()
    {
#if UNITY_EDITOR
        if (TestManager.Instance.getRewardDirect)
        {
            return true;
        }
#endif

        if ((null!=this.rewardedAd) && this.rewardedAd.CanShowAd())
        {
            return true;
        }
        return false;
       

    }

    override protected void showRewardAd()
    {

#if UNITY_EDITOR
        if (TestManager.Instance.getRewardDirect)
        {
            base.HandleUserEarnedReward("testAD - from - CSJAD-rewardVideoAd complete");//加载成功回调事件
            return;
        }
#endif

        if ((null != this.rewardedAd) && this.rewardedAd.CanShowAd())
        {
            base.showRewardAd();
            ShowRewardedAd();
        }
        else
        {
            HandleRewardedAdFailedToShowAdmob();
        }
    }

    private void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                HandleUserEarnedRewardAdmob();
            });
        }
    }


    public void HandleRewardedAdLoadedAdmob()
    {
        base.HandleRewardedAdLoaded("HandleRewardedAdLoadedAdmob event received");

    }

   
    //加载失败这里不能自动重新请求；否则会连续失败；
    //要用户点请求后再去请求，最好有加载失败的原因
    public void HandleRewardedAdFailedToLoadAdmob()
    {
        base.HandleRewardedAdFailedToLoad("HandleRewardedAdFailedToLoadAdmob");

    }


    //有open就会有close
    public void HandleRewardedAdOpeningAdmob()
    {
        base.HandleRewardedAdOpening("HandleRewardedAdOpeningAdmob");


    }

    public void HandleRewardedAdFailedToShowAdmob()
    {
        base.HandleRewardedAdFailedToShow("HandleRewardedAdFailedToShowAdmob");
    }


   

    public void HandleRewardedAdClosedAdmob()
    {

        base.HandleRewardedAdClosed("HandleRewardedAdClosed");
    }


    
    public void HandleUserEarnedRewardAdmob()
    {
        

        base.HandleUserEarnedReward("HandleRewardedAdRewarded event received for ");


    }

#endif

}
