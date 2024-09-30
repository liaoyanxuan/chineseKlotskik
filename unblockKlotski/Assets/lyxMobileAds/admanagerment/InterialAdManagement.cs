using System;
using System.Collections;
using System.Collections.Generic;
using sw.util;
using UnityEngine;

public class InterialAdManagement
{
    private List<IAdWorker> _adWorksList;

    private static InterialAdManagement _instance;
    public static InterialAdManagement getInstance
    {
        get
        {
            if (null != _instance)
            {
                return _instance;
            }
            _instance = new InterialAdManagement();
            return _instance;
        }

    }

    private InterialAdManagement()
    {
        _adWorksList = new List<IAdWorker>();


#if UNITY_ANDROID

        //简体中文

#if GOOGLEPLAY
                _adWorksList.Add(AdmobInterstitialAdScript.Instance);  //Admob
               
#endif


#elif UNITY_IOS


            _adWorksList.Add(AdmobInterstitialAdScript.Instance);  //Admob


#endif


        requireAds();


    }


    public bool isInterstitialAvailable()
    {
        for (int i = 0; i < _adWorksList.Count; i++)
        {
            if (_adWorksList[i].isAdReady() == true)
            {
                return true;
            }
        }
        requireAds();
        return false;
    }

    public void requireAds()
    {
        for (int i = 0; i < _adWorksList.Count; i++)
        {
            if (_adWorksList[i].isAdReady() == false)
            {
                _adWorksList[i].CreateAndLoadAd();  //请求
            }
        }
    }


    private int _currentWorkerIndex = 0;
    private Action<bool> _currentSussCallBack;
   
    private IAdWorker _CurrentWorker;
    private string _currentAdType = string.Empty;
    public void UserChoseToWatchAd(string adType, Action<bool> successActionCallBack, Action<bool> closeCallBack)
    {
        _currentAdType = adType;

        requireAds();

        _currentSussCallBack = successActionCallBack;
       

        _currentWorkerIndex = _currentWorkerIndex % _adWorksList.Count;

        Debug.Log("adtest-Interial->_currentWorkerIndex_2:" + _currentWorkerIndex);

        _CurrentWorker = _adWorksList[_currentWorkerIndex];

       
        int tryTime = 0;
        while (_CurrentWorker.isAdReady()==false&& tryTime<_adWorksList.Count)
        {
            tryTime++;
            _currentWorkerIndex++;
            _currentWorkerIndex = _currentWorkerIndex % _adWorksList.Count;
            _CurrentWorker = _adWorksList[_currentWorkerIndex];
        }

        if (_CurrentWorker.isAdReady())
        {
            _CurrentWorker.UserChoseToWatchAd(adType, sussToReward, null, null, closeCallBack);
        }

        _currentWorkerIndex++;

    }


    //成功获取
    private void sussToReward(bool getWard,string adtype)
    {
        Debug.Log("adtest->sussToReward");
        _CurrentWorker = null;
      
        if (null != _currentSussCallBack)
        {
            _currentSussCallBack(getWard);
        }
    }

   


}