using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*1.有成功和失败回调；
*2.有超时设置
*/
public interface IAdWorker
{
    //用户请求看广告
	void UserChoseToWatchAd(string adType, Action<bool,string> actionCallBack, Action<bool> failActionCallBack, Action<bool> requireNextAdCallBack, Action<bool> closeCallBack);
    void CreateAndLoadAd(bool needErrorTip = false);
    void setCallBack(Action<bool, string> actionCallBack, Action<bool> failActionCallBack, Action<bool> requireNextAdCallBack);
    bool isAdReady();
    



}
