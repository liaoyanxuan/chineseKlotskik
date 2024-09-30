#if GOOGLEPLAY || UNITY_IOS
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

//谷歌广告插件
public class AdmobMobileBannerAdsScript : BaseBannerAdScript
{
#if GOOGLEPLAY || UNITY_IOS
    private static AdmobMobileBannerAdsScript instance;

    //单例模式
    public static AdmobMobileBannerAdsScript Instance
    {
        get
        {
            if (null != instance)
            {
                return instance;
            }
            AdmobMobileBannerAdsScript[] _monoComponents = FindObjectsOfType(typeof(AdmobMobileBannerAdsScript)) as AdmobMobileBannerAdsScript[];
            instance = _monoComponents[0];
            return instance;
        }

    }


    private bool isBannerShowing = true;

    public bool IsBannerShowing
    {
        get { return isBannerShowing; }

    }

    private BannerView bannerView;


    override protected void OnStart()
    {
        base.OnStart();
       
    }


    private void initBanner()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        if (bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            bannerView.Destroy();
            bannerView = null;
        }

        string bannerAdUnitId = GoogleAdmobID.BannerAdUnitId;

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(bannerAdUnitId, new AdSize(320, 50), AdPosition.Bottom);


    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
            HandleOnAdLoaded();
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
            HandleOnAdFailedToLoad();
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
            HandleOnAdOpened();
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
            HandleOnAdClosed();
        };
    }



    override public void showBanner()
    {
        if (bannerView == null)
        {
            initBanner();
        }

        if (bannerView != null)
        {
            // Create an empty ad request.
            AdRequest request = GoogleAdmobID.getAdRequest();

            // Load the banner with the request.
            bannerView.LoadAd(request);

            bannerView.Show();
            isBannerShowing = true;

            MonoBehaviour.print(this.GetType().Name + " 展示广告，showAd");
        }
    }

    public void hideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
            isBannerShowing = false;
        }
    }

    public void HandleOnAdLoaded()
    {
      
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad()
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received");
    }

    public void HandleOnAdOpened()
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed()
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication()
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
#endif

}
