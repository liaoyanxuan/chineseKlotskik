using System.Collections;
using System.Collections.Generic;
using Hyperbyte.Ads;
using UnityEngine;

public class AdInitAndCache : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      

        if (AdManager.Instance.adSettings.rewardedAdsEnabled)
        {
            AdManagement adManagement = AdManagement.getInstance;
        }



        if (AdManager.Instance.adSettings.interstitialAdsEnabled)
        {
            InterialAdManagement interialAdManagement = InterialAdManagement.getInstance;
        }

        if (AdManager.Instance.adSettings.bannerAdsEnabled)
        {
            BannerAdAdManagement bannerAdAdManagement = BannerAdAdManagement.getInstance;
        }

        AdManager.adobjectInit = true;

    }

    // Update is called once per frame

}
