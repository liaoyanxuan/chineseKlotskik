using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sw.util;
using Hyperbyte.Localization;

public class AdManagement
{
	private List<IAdWorker> _adWorksList;

	private static AdManagement _instance;
	public static AdManagement getInstance
	{
		get
		{
			if (null != _instance)
			{
				return _instance;
			}
            _instance = new AdManagement();
			return _instance;
		}
	}


	private AdManagement()
    {

        _adWorksList = new List<IAdWorker>();

#if UNITY_ANDROID


    #if GOOGLEPLAY
            _adWorksList.Add(AdmobRewardAdScript.Instance);  //Admob
    #endif




#elif UNITY_IOS

        
            _adWorksList.Add(AdmobRewardAdScript.Instance);  //Admob

#endif




        foreach (IAdWorker iWorker in _adWorksList)
        {
            iWorker.setCallBack(sussToReward, failToLoadCallBack, requireNextAdCallBack);
        }

        initSetting.Instance.StartCoroutine(createAndLoadAdFirst());

        
    }

    IEnumerator createAndLoadAdFirst()
    {
        yield return null;
        _adWorksList[0].CreateAndLoadAd();  //初始请求
        yield return null;
    }


    private int MAX_TRY = 3;
    private int _currentWorkerIndex = 0;
    private string _currentADType;
    private Action<bool, string> _currentSussCallBack;
    private int _tryTime = 0;
    private IAdWorker _CurrentWorker;
    public void UserChoseToWatchAd(string adType, Action<bool,string> successActionCallBack)
    {

        _currentADType = adType;
        _currentSussCallBack = successActionCallBack;

        if (null!=_CurrentWorker)
        {
            //Worker可处理重复请求
            _CurrentWorker.UserChoseToWatchAd(adType, sussToReward, failToLoadCallBack, requireNextAdCallBack,null);
            return;
        }

        DebugEx.Log("adtest->_currentWorkerIndex_1:" + _currentWorkerIndex);

        _currentWorkerIndex = _currentWorkerIndex % _adWorksList.Count;

        DebugEx.Log("adtest->_currentWorkerIndex_2:" + _currentWorkerIndex);

         _CurrentWorker = _adWorksList[_currentWorkerIndex];

        _currentWorkerIndex++;

        _CurrentWorker.UserChoseToWatchAd(adType, sussToReward, failToLoadCallBack, requireNextAdCallBack,null);

    }


    public bool isRewardedAvailable()
    {
        _currentWorkerIndex = _currentWorkerIndex % _adWorksList.Count;
        bool isReady= _adWorksList[_currentWorkerIndex].isAdReady();
        if (isReady == false)
        {
            requireNextAdCallBack(false);
        }

        return isReady;
    }

  

    private int requireNextFailCount;
    //请求缓存队列下一个视频
    private void requireNextAdCallBack(bool isFailed)
    {

        _CurrentWorker = null;

        if (isFailed)
        {
            _currentWorkerIndex++;
            requireNextFailCount++;
        }
        else
        {
            requireNextFailCount = 0;
        }

       
        if (requireNextFailCount > _adWorksList.Count)
        {
            requireNextFailCount = 0;  //不再继续
        }
        else
        {
            _currentWorkerIndex = _currentWorkerIndex % _adWorksList.Count;
            DebugEx.Log("adtest->requireNextAdCallBack:" + _currentWorkerIndex);
            _adWorksList[_currentWorkerIndex].CreateAndLoadAd();
        }
    }

    //成功获取
    private void sussToReward(bool getWard,string adType)
    {
        Debug.Log("adtest->sussToReward");
        _CurrentWorker = null;
        _tryTime = 0;
        if (null != _currentSussCallBack)
        {
            _currentSussCallBack(getWard, adType);
        }
    }


    //失败回调
    private void failToLoadCallBack(bool getWard)
    {
        _CurrentWorker = null;

        if (getWard)   //非断网才重试
        {
            _tryTime++;

            Debug.Log("adtest->failToLoadCallBack-_tryTime:" + _tryTime);
            if (_tryTime < _adWorksList.Count && _tryTime < MAX_TRY)
            {
                UserChoseToWatchAd(_currentADType, _currentSussCallBack);
            }
            else  //尝试次数达到最大
            {
                // sussToReward(true, _currentADType);  //成功
                _tryTime = 0;
                Toast.instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag(Language.advertisementLoadFail));
            }

        }
        else
        {
            _tryTime = 0;
            Toast.instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag(Language.advertisementLoadFail));
        }

    }



}
