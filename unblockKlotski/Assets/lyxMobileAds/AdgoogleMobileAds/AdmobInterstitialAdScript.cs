#if GOOGLEPLAY || UNITY_IOS
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

//谷歌广告插件
public class AdmobInterstitialAdScript : BaseInterstitialAdScript
{
#if GOOGLEPLAY || UNITY_IOS
    public static AdmobInterstitialAdScript instance;

    //单例模式
    public static AdmobInterstitialAdScript Instance
    {
        get
        {
            if (null != instance)
            {
                return instance;
            }
            AdmobInterstitialAdScript[] _monoComponents = FindObjectsOfType(typeof(AdmobInterstitialAdScript)) as AdmobInterstitialAdScript[];
            instance = _monoComponents[0];
            return instance;
        }

    }


    override protected  void OnStart()
    {

        // Initialize the Google Mobile Ads SDK.

        MobileAds.Initialize(initStatus => { });
    }

   
   
  

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////
    /// 激励广告
    /// </summary>
    private InterstitialAd interstitialAd;
    override public void CreateAndLoadAd(bool needErrorTip = false)
    {

        if (LoadState.LOADING == loadState)
        {
            Debug.LogError("AdmobInterstitial--LoadState.LOADING");
            return;
        }

        DisposeAds();//销毁之前的


        base.CreateAndLoadAd(needErrorTip);

        string interstitialAdUnitID = GoogleAdmobID.InterstitialAdUnitID;


        AdRequest adRequest = GoogleAdmobID.getAdRequest();

       

        // send the request to load the ad.
        InterstitialAd.Load(interstitialAdUnitID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    HandleInterstitialAdFailedToLoad();
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
                RegisterEventHandlers(interstitialAd);

                HandleInterstitialAdLoaded();
            });


    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
            HandleInterstitialAdOpening();
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            HandleInterstitialAdClosed();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            CreateAndLoadAd();
        };
    }


    private void DisposeAds()
    {
        if (this.interstitialAd != null)
        {
            alreadyShow = false;
            loadState = LoadState.NONE;
            this.interstitialAd.Destroy(); //销毁旧的
            this.interstitialAd = null;
        }
    }


    //广告是否准备好
    override public bool isAdReady()
    {

        //如果已经show过了，但没有调用到DisposeAds（）,补充调用；
        if (alreadyShow == true)
        {
            DisposeAds();
        }

        if (this.interstitialAd!=null)
        {
            return this.interstitialAd.CanShowAd();
        }
        else
        {
            return false;
        }
        
    }

    private bool alreadyShow = false;
    //播放广告
    override protected void showAd()
    {
        alreadyShow = true;
        this.interstitialAd.Show();
    }


    //有open就会有close
    public void HandleInterstitialAdOpening()
    {
        MonoBehaviour.print("Admob->HandleInterstitialAdOpening event received");
        base.adSussOpenCallBack();
    }


    public void HandleInterstitialAdLoaded()
    {
        MonoBehaviour.print("Admob->HandleInterstitialAdLoaded event received");
        loadState = LoadState.LOADSUCC;
    }

   
    //加载失败这里不能自动重新请求；否则会连续失败；
    //要用户点请求后再去请求，最好有加载失败的原因
    public void HandleInterstitialAdFailedToLoad()
    {
        MonoBehaviour.print(
            "Admob->HandleInterstitialAdFailedToLoad event received");

        loadState = LoadState.LOADFAIL;
    }


    public void HandleInterstitialAdClosed()
    {
        MonoBehaviour.print("Admob->HandleInterstitialAdClosed");
        CreateAndLoadAd();
    }

   

    public void HandleOnInterstitialAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("Admob->HandleInterstitialAdLeavingApplication event received");
    }
#endif

}
