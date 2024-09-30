#if  UNITY_IOS
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using Hyperbyte;
#endif
using UnityEngine;

public class AdmobOpenAdScript : MonoBehaviour
{
#if  UNITY_IOS
	public void Start()
	{
		MobileAds.Initialize(initStatus => { });
		// Load an app open ad when the scene starts
		AppOpenAdManager.Instance.LoadAd();

		// Listen to application foreground and background events.
		AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
	}

	private void OnAppStateChanged(AppState state)
	{
		// Display the app open ad when the app is foregrounded.
		UnityEngine.Debug.Log("App State is " + state);
		
		//去广告后不显示
			if (ProfileManager.Instance.IsAppAdFree())
			{
				return;
			}
			
		if (state == AppState.Foreground)
		{
			AppOpenAdManager.Instance.ShowAdIfAvailable();
		}
	}
#endif
}
