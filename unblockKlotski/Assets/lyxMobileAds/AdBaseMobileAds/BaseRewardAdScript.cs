using Hyperbyte.Localization;
using sw.util;
using System;
using System.Collections;
using UnityEngine;


public enum AD_TYPE
{
    NONE,
    NEW_GAME,
    CHANGE_SHAPE,
    MISS_CAND,
    HINT,
    UNLOCKED_GAME,
    ERROR_TIME_CLEAR, //错误次数清零
    CUSOMER_ANSWER,// 自定义题的答案


}
//激励广告基类
public class BaseRewardAdScript : MonoBehaviour, IAdWorker
{
	
       
    protected LoadState _loadState = LoadState.NONE;

    private bool loadToShow = false;
    protected DateTime loadTime;

    private string _adType= string.Empty;

    private Action<bool, string> _sussActionCallBack;
    private Action<bool> _failActionCallBack;
    private Action<bool> _requireNextAdCallBack;

    private Coroutine _loadingTimeCoroutine;

    private bool isNeedErrorTip = false;

    protected enum LoadState 
    {
        NONE,
        LOADING,
        LOADFAIL,
        LOADSUCC
    }

    public void Start()
    {
        OnStart();
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

            if (_loadState != LoadState.LOADING)
            {
                //非loading有结果，停止计时
                if (null != _loadingTimeCoroutine)
                {
                    StopCoroutine(_loadingTimeCoroutine);
                    _loadingTimeCoroutine=null;
                }
            }
        }
    }

    private bool _getRewordBool = false;  //获得奖励
    private bool _failedToLoadBool = false; //加载失败 failToLoad
    private bool _failedToShowBool = false;  //展示失败 fialToShow
    private bool _AdClosedBool = false;  //关闭
    private bool _failAndRequireAgain = false;  //失败且需要重新请求（需要区分是观看请求还是缓存）

    public void Update()
    {
        if (_getRewordBool)
        {
            _getRewordBool = false;
            getReWard();
           
        }


        if (_failedToLoadBool)
        {
            _failedToLoadBool = false;
            failedtoLoadDone(); 
        }

        if (_failedToShowBool)
        {
            _failedToShowBool = false;
            faileToShowDone();
        }

        if (_AdClosedBool)
        {
            _AdClosedBool = false;
            AdClosedDone();
        }

        if (_failAndRequireAgain)
        {
            _failAndRequireAgain = false;
            failAndRequireAgainDone();
        }

       
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////
    /// 激励广告
    /// </summary>
   //请求--->加载广告
    virtual public void CreateAndLoadAd(bool needErrorTip = false)
    {

        MonoBehaviour.print(this.GetType().Name + " CreateAndLoadRewardedAd");
        isNeedErrorTip = needErrorTip;

        clearState();
        loadState = LoadState.LOADING; //30秒后清理状态,只有1个地方设置LOADING

        if (null != _loadingTimeCoroutine)
        {
            StopCoroutine(_loadingTimeCoroutine);
            _loadingTimeCoroutine = null;
        }
        
    
        _loadingTimeCoroutine = StartCoroutine(loadingTime());  //倒计时
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
     virtual protected void showRewardAd()
    {
        loadToShow = false;
        MonoBehaviour.print(this.GetType().Name + " 展示广告，showRewardAd");
    }




    //回调--->加载成功
    virtual protected void HandleRewardedAdLoaded(string message)
    {
        MonoBehaviour.print(this.GetType().Name + message);
        loadState = LoadState.LOADSUCC;
        loadTime = DateTime.UtcNow;

        if (true == loadToShow)   //自动播
        {
            loadToShow = false;
            showRewardAd();//展示广告
        }
        else
        {
            Hyperbyte.Ads.AdManager.Instance.OnRewardedLoaded();//加载成功回调事件
        }

        
    }

  

    //回调--->加载失败
    //加载失败这里不能自动重新请求；否则会连续失败；
    //要用户点请求后再去请求，最好有加载失败的原因
    virtual protected void HandleRewardedAdFailedToLoad(string message)
    {
        MonoBehaviour.print(this.GetType().Name + " HandleRewardedAdFailedToLoad: " + message);

        
        _failedToLoadBool = true;
          
    }

    //update调用--->加载失败
    virtual protected void failedtoLoadDone()
    {
        loadState = LoadState.LOADFAIL;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //网络不可用
            MonoBehaviour.print(
            this.GetType().Name + " HandleRewardedAdFailedToLoad 明确是网络不可用，不给奖励 ");
            //提示：广告请求失败，请检查网络后重试
            Toast.instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag(Language.advertisementLoadFail));

        }

        _failAndRequireAgain = true;

    }


    //有open就会有close
    //回调--->打开广告
    virtual protected void HandleRewardedAdOpening(string message)
    {
        MonoBehaviour.print(this.GetType().Name + " HandleRewardedAdOpening event received:" + message);
      
    }

    //回调--->展示失败
    virtual protected void HandleRewardedAdFailedToShow(string message)
    {
        MonoBehaviour.print(
            this.GetType().Name + " HandleRewardedAdFailedToShow event received with message: "
                             + message);

        _failedToShowBool = true;
      
    }

    //update--->展示失败
    virtual protected void faileToShowDone()
    {
        //无法展示，重新请求，无法展示也就无close
        //提示：广告无法展示，请重试
        Toast.instance.ShowMessage(Language.getInstance().GetValue(Language.advertisementOpenFail));
        _failAndRequireAgain = true;

    }


    //回调--->关闭
    public void HandleRewardedAdClosed(string message)
    {
        MonoBehaviour.print(message);
       
        _AdClosedBool = true;
    }


    //update--->关闭
    private void AdClosedDone()
    {
        //关闭后，重新请求,应该请求队列下一个
       // this.CreateAndLoadRewardedAd();

        if (null != _requireNextAdCallBack)
        {
            _requireNextAdCallBack(false);//重新请求,应该请求队列下一个
        }
        MonoBehaviour.print(this.GetType().Name + " HandleRewardedAdClosed event received");

        Hyperbyte.Ads.AdManager.Instance.OnRewardedClosed();
    }


    //回调--->获得奖励
    public void HandleUserEarnedReward(string message)
    {
        
        MonoBehaviour.print(message);
        _getRewordBool = true;  //非主线程的调用
    }


    //update--->获得奖励
    private void getReWard()
    {
       
        if (_sussActionCallBack != null) 
        {
            _sussActionCallBack(true, _adType);
            _sussActionCallBack = null;
        }

       

        _adType = string.Empty;

    }


    public void setCallBack(Action<bool, string> sussActionCallBack, Action<bool> failActionCallBack, Action<bool> requireNextAdCallBack)
    {
        _sussActionCallBack = sussActionCallBack;
        _failActionCallBack = failActionCallBack;
        _requireNextAdCallBack = requireNextAdCallBack;
    }


    //用户请求播放广告
    public void UserChoseToWatchAd(string adType,Action<bool,string> sussActionCallBack, Action<bool> failActionCallBack, Action<bool> requireNextAdCallBack, Action<bool> closeAdCallBack)
    {
        isNeedErrorTip = true;
        _adType = adType;
        _sussActionCallBack = sussActionCallBack;
        _failActionCallBack = failActionCallBack;
        _requireNextAdCallBack = requireNextAdCallBack;
      

        if (isAdReady())  //loadsuss,是否准备好
        {
            loadToShow = false;
            showRewardAd();

        }
        else
        {
            //在加载中； isLoading；   
            loadToShow = true;  //load完自动打开
                                //没准备好的情况，只需要处理两种情况
            if (LoadState.LOADING == loadState)
            {

            }
            else
            {  //之前加载失败过，重新请求，此时转为状态loading

                this.CreateAndLoadAd(true);

            }

            //提示：正在加载ad，请稍后
            Toast.instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag(Language.isloadingAdvertisement));

            //not ready
            MonoBehaviour.print(this.GetType().Name + " Rewarded ad is not ready yet");
        }
        
    }


    private IEnumerator loadingTime()
    {
#if UNITY_ANDROID
        yield return new WaitForSeconds(30);  //30秒超时
#else
        yield return new WaitForSeconds(30);  //30秒超时,iOS和其他
#endif

        if (string.IsNullOrEmpty(_adType) == false)   //请求了奖励
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //网络不可用
                MonoBehaviour.print(
                "HandleRewardedAdFailedToLoad 明确是网络不可用，不给奖励 ");

                Toast.instance.ShowMessage(LocalizationManager.Instance.GetTextWithTag(Language.advertisementLoadFail));
       
            }
            else
            {
                if (LoadState.LOADING == loadState)   //状态还是在loading
                {
                    MonoBehaviour.print(
                        this.GetType().Name+" HandleRewardedAd loading timeout !!");  
                }
            }
        }

        _loadingTimeCoroutine = null;
        clearState(); //无论如何，清理状态

        _failAndRequireAgain = true;
    }

    private void failAndRequireAgainDone()
    {
       
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                failActionHandler(false);  //无网络，不给奖励,不算尝试次数
            }
            else
            {
                    if (loadToShow)
                    {
                        loadToShow = false;
                        failActionHandler(true);    ////请求下一个，要求马上播
                    }
                    else
                    {
                            if (null != _requireNextAdCallBack)
                            {
                                _requireNextAdCallBack(true);////请求下一个，仅仅缓存
                            }
                    }
                
            }
        
    }


    private void failActionHandler(bool reward)
    {
        clearState();
        if (null != _failActionCallBack)
        {
            _failActionCallBack(reward);
        }
    }

    private void clearState() 
    {
        loadState = LoadState.NONE;
        
    }

}