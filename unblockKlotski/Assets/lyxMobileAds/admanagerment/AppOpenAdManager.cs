using System;
#if GOOGLEPLAY || UNITY_IOS
using GoogleMobileAds.Api;
#endif
using UnityEngine;

public class AppOpenAdManager
{
#if GOOGLEPLAY || UNITY_IOS
#if UNITY_ANDROID
    private const string AD_UNIT_ID = "ca-app-pub-4488157848198084/8197401983";
#elif UNITY_IOS
	private const string AD_UNIT_ID = "ca-app-pub-4488157848198084/6113862800";
#else
	private const string AD_UNIT_ID = "unexpected_platform";
#endif

	private static AppOpenAdManager instance;

	private DateTime loadTime;
	private AppOpenAd ad;

	private bool isShowingAd = false;

	public static AppOpenAdManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AppOpenAdManager();
			}

			return instance;
		}
	}

	private bool IsAdAvailable
	{
		get
		{
			return ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
		}
	}

	public void LoadAd()
	{

#if UNITY_IOS
		iOSUtil.loadOpenAd();
#else
		AdRequest request = new AdRequest.Builder().Build();

		// Load an app open ad for portrait orientation
		AppOpenAd.LoadAd(AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
		{
			if (error != null)
			{
				// Handle the error.
				Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
				return;
			}

			// App open ad is loaded.
			ad = appOpenAd;
			loadTime = DateTime.UtcNow;
		}));
#endif
	}

	public void ShowAdIfAvailable()
	{
#if UNITY_IOS
		iOSUtil.presentOpenAd();
#else
		if (!IsAdAvailable || isShowingAd)
		{
			LoadAd();
			return;
		}

		ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
		ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
		ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
		ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
		ad.OnPaidEvent += HandlePaidEvent;

		
		ad.Show();
#endif
	}

	private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
	{
		Debug.Log("Closed app open ad");
		// Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
		if (ad!=null)
		{
			ad.Destroy();
		}
		ad = null;
		isShowingAd = false;
		LoadAd();
	}

	private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
	{
		Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
		// Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
		if (ad != null)
		{
			ad.Destroy();
		}
		ad = null;
		LoadAd();
	}

	private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
	{
		Debug.Log("Displayed app open ad");
		isShowingAd = true;
	}

	private void HandleAdDidRecordImpression(object sender, EventArgs args)
	{
		Debug.Log("Recorded ad impression");
	}

	private void HandlePaidEvent(object sender, AdValueEventArgs args)
	{
		Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
				args.AdValue.CurrencyCode, args.AdValue.Value);
	}
#endif
	}