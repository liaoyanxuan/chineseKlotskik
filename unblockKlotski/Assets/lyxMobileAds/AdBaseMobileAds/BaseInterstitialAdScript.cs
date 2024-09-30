using System;
using System.Collections;
using System.Collections.Generic;
using sw.util;
using UnityEngine;

public class BaseInterstitialAdScript : MonoBehaviour, IAdWorker
{

    private Action<bool, string> _actionCallBack;
    private Action<bool> _closeCallBack;
    private string _adType = string.Empty;
    private bool _getRewordBool = false;
    // Start is called before the first frame update
    protected DateTime loadTime;
    protected enum LoadState
    {
        NONE,
        LOADING,
        LOADFAIL,
        LOADSUCC
    }

    protected LoadState _loadState = LoadState.NONE;
    private Coroutine _loadingTimeCoroutine;

    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {

        if (_getRewordBool)
        {
            _getRewordBool = false;
            getReWard();

        }
       
    }


    protected virtual void OnStart()
    {
        //初始化
    }

   


    protected LoadState loadState
    {
        get
        {
            return _loadState;
        }

        set
        {
            _loadState = value;

            DebugEx.Log("<Unity Log>...BaseInterstitialAdScript-LoadState:"+ _loadState);

            if (_loadState != LoadState.LOADING)
            {
                //非loading有结果，停止计时
                if (null != _loadingTimeCoroutine)
                {
                    StopCoroutine(_loadingTimeCoroutine);
                    _loadingTimeCoroutine = null;
                }
            }
        }
    }


    private void getReWard()
    {
        if (_actionCallBack != null)
        {
            _actionCallBack(true, _adType);
        }

        _actionCallBack = null;
    }

    public void setCallBack(Action<bool, string> actionCallBack, Action<bool> failActionCallBack, Action<bool> requireNextAdCallBack) { }


    //用户请求看广告
    public void UserChoseToWatchAd(string adType, Action<bool,string> actionCallBack, Action<bool> failActionCallBack, Action<bool> requireNextAdCallBack, Action<bool> closeAdCallBack)
    {
        _adType = adType;
        _actionCallBack = actionCallBack;
        _closeCallBack = closeAdCallBack;

      
        if (isAdReady())  //目前已经准备好，立即显示，插屏必须立即显示
        {
            showAd();
        }
        else
        {
            this.CreateAndLoadAd();

            //加载失败；loadFail；
            DebugEx.Log("InterstitialAd ad is not ready yet");
        }
        
    }



    virtual public void CreateAndLoadAd(bool needErrorTip = false)
    {
        clearState();
        loadState = LoadState.LOADING; //30秒后清理状态,只有1个地方设置LOADING

        if (null != _loadingTimeCoroutine)
        {
            StopCoroutine(_loadingTimeCoroutine);
            _loadingTimeCoroutine = null;
        }


        _loadingTimeCoroutine = StartCoroutine(loadingTime());  //倒计时

    }

    private IEnumerator loadingTime()
    {

        DebugEx.Log("<Unity Log>...BaseInterstitialAdScript-loadingTime");
        yield return new WaitForSeconds(30);  //30秒超时
        if (null != _loadingTimeCoroutine)
        {
            StopCoroutine(_loadingTimeCoroutine);
            _loadingTimeCoroutine = null;
        }
        clearState(); //无论如何，清理状态

    }

    private void clearState()
    {
        DebugEx.Log("<Unity Log>...BaseInterstitialAdScript-clearState");
        loadState = LoadState.NONE;

    }


    //回调--->加载成功
    virtual protected void HandleRewardedAdLoaded(string message)
    {
        DebugEx.Log(this.GetType().Name + message);
        loadState = LoadState.LOADSUCC;
        loadTime = DateTime.UtcNow;

    }

    //广告是否准备好
    virtual public bool isAdReady()
	{
        if (loadState == LoadState.LOADSUCC && (System.DateTime.UtcNow - loadTime).TotalHours < 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	//展示广告,子类必须调用，设置loadToshow为false，是否要求立即播放
	virtual protected void showAd()
	{

        DebugEx.Log(this.GetType().Name + " 展示广告，showAd");
	}


    //有open就会有close
    public void adSussOpenCallBack()
    {
        DebugEx.Log("HandleInterstitial-->adSussOpenCallBack");
        _getRewordBool = true;  //非主线程的调用
    }

    public void closeCallBack()
    {
        if (_closeCallBack != null)
        {
            _closeCallBack(true);
        }
       
    }


}
