using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScaler : MonoBehaviour {

    private static CameraScaler instance;
    //竖屏游戏
    [SerializeField]
    private float devHeight = 1920f; //设计分辨率

    [SerializeField]
    private float devWidth = 1080f; //设计分辨率
    
	// Use this for initialization

    //单例模式
    public static CameraScaler Instance
    {
        get
        {
            CameraScaler[] _monoComponents = FindObjectsOfType(typeof(CameraScaler)) as CameraScaler[];
            instance = _monoComponents[0];
            return instance;
        }
    }

    void Awake() 
    {
        
        //实际屏幕宽高比
        float aspectRatio = Screen.width * 1.0f / Screen.height;

        float cameraWidth = devHeight * aspectRatio;

        

        CanvasScaler canvasScaler = (CanvasScaler)this.GetComponent<CanvasScaler>();
        if (cameraWidth < devWidth)  //当前摄像机高度不够，导致宽度无法完整显示
        {
           //确保宽度
            canvasScaler.matchWidthOrHeight = 0;
        }
        else    //宽度够用，高度不够用
        {  //确保高度
            canvasScaler.matchWidthOrHeight = 1;
        }

    }


    //屏幕截图
    public Rect getMainRect() 
    {
       

		Rect rect;

		
		rect=new Rect(0, 0, Screen.width, Screen.height);

		
		
		return rect;
        
    }

}
