using System;
using System.Collections;
using System.Collections.Generic;
using sw.util;
using UnityEngine;

public class BaseBannerAdScript : MonoBehaviour
{

    public int tryTimes = 0;
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
  


    protected virtual void OnStart()
    {
        //初始化
    }

   


    //展示广告,子类必须调用，设置loadToshow为false，是否要求立即播放
    virtual public void showBanner()
	{
		
		MonoBehaviour.print(this.GetType().Name + " showBanner");
	}

    virtual public void hideBanner()
    {

        MonoBehaviour.print(this.GetType().Name + " hideBanner");
    }

}
