using System;
using System.Collections;
using System.Collections.Generic;
using Hyperbyte.Ads;
using sw.util;
using UnityEngine;

public class BannerAdAdManagement
{
    private List<BaseBannerAdScript> _baseBannerAdScriptList;

    private static BannerAdAdManagement _instance;
    public static BannerAdAdManagement getInstance
    {
        get
        {
            if (null != _instance)
            {
                return _instance;
            }
            _instance = new BannerAdAdManagement();
            return _instance;
        }

    }

    private int CSJ_BANNER_RATE=10;
    private BannerAdAdManagement()
    {
        _baseBannerAdScriptList = new List<BaseBannerAdScript>();

#if UNITY_EDITOR && UNITYAD
        _baseBannerAdScriptList.Add(UnityBannerAdScript.Instance);  //unity
#endif

#if UNITY_ANDROID


#if GOOGLEPLAY
                  _baseBannerAdScriptList.Add(AdmobMobileBannerAdsScript.Instance);  //Admob         
#endif

#if CSJAD
                if (UnityEngine.Random.Range(0, 10) < CSJ_BANNER_RATE)
                {
                    // _baseBannerAdScriptList.Add(TCendUnityBannerAdScript.Instance);
                    _baseBannerAdScriptList.Add(AdCSJBannerAdScript.Instance);
                }
                else
                {
                    _baseBannerAdScriptList.Add(TCendUnityBannerAdScript.Instance);
                }
#endif

#if CSJADJH
                 _baseBannerAdScriptList.Add(AdCSJJHBannerAdScript.Instance);  //csjad
#endif

#if UNITYAD
                 _baseBannerAdScriptList.Add(UnityBannerAdScript.Instance);  //unity
#endif



#if BYTEDGAME
        _baseBannerAdScriptList.Add(BDBannerAdScript.Instance);  //字节小游戏
#endif


#elif UNITY_IOS


            _baseBannerAdScriptList.Add(AdmobMobileBannerAdsScript.Instance);  //Admob

         
        

#elif UNITYAD
        _baseBannerAdScriptList.Add(UnityBannerAdScript.Instance);  //unity
#endif


       

    }


    IEnumerator refreshBanner()
    {
        while (true)
        {
            //间隔180秒刷新一次,3分钟
            yield return new WaitForSeconds(180);
            showBanner();
        }
    }  

    private int _currentWorkerIndex = 0;
    private BaseBannerAdScript _CurrentWorker;
    private bool initRefresh = false;
    public void showBanner()
    {

        if (!initRefresh)
        {
            initRefresh = true;
            initSetting.Instance.StartCoroutine(refreshBanner());
        }

        DebugEx.Log("showBanner()-4");


        _currentWorkerIndex = _currentWorkerIndex % _baseBannerAdScriptList.Count;

        Debug.Log("adtest-banner->_currentWorkerIndex_2:" + _currentWorkerIndex);

        _CurrentWorker = _baseBannerAdScriptList[_currentWorkerIndex];
 
        _CurrentWorker.showBanner();
       
        _currentWorkerIndex++;

    }

    public void hideBanner()
    {
        for (int i = 0; i < _baseBannerAdScriptList.Count; i++)
        {
            _baseBannerAdScriptList[i].hideBanner();
        }
    }

}