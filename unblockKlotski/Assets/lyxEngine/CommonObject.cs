using liaoyanxuan.common.injector;
using liaoyanxuan.common.interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CommonObject  {

    
    [Inject(InjectType.SINGLETON)]
    protected ILoggerHelper iLoggerHelper { get; set; }    //日志系统;    负责写日志

    [Inject(InjectType.SINGLETON)]
    protected IAssetLoader iAssetLoader { get; set; }        //asset资源加载

    protected CommonObject()
    {
        InjectorFactory.Instance.Inject(this);
    }
}
