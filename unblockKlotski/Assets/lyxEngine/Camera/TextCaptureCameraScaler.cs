using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCaptureCameraScaler : MonoBehaviour
{
     [SerializeField]
    private RenderTexture rt;

    [SerializeField]
    private GameObject iOSQRCodeObject;
    [SerializeField]
    private GameObject googlePlayQRCodeObject;
    [SerializeField]
    private GameObject androidQRCodeObject;

    private static TextCaptureCameraScaler instance;
    //竖屏游戏
    float devHeight = 1334f; //设计分辨率
    float devWidth = 750f; //设计分辨率

    int captureHeight = 210;
    int captureWidth = 750;

	private bool awakeInit=false;

	// Use this for initialization

    //单例模式
    public static TextCaptureCameraScaler Instance
    {
        get
        {
			if (null != instance) 
			{
				return instance;
			}

			TextCaptureCameraScaler[] _monoComponents = FindObjectsOfType(typeof(TextCaptureCameraScaler)) as TextCaptureCameraScaler[];
            instance = _monoComponents[0];
			instance.Awake (); 
            return instance;
        }
    }

	public void disable()
	{
		this.GetComponent<Camera> ().enabled = false;
		this.gameObject.SetActive (false);

	}


    void Awake() 
    {
		if (true == awakeInit)
			return;
		awakeInit = true;

      
        float screenHeight = Screen.height;
        

        float orthographicSize = devHeight / 2; //最低高度,确保可以显示此高度，以此为基础计算可显示摄像机宽度；

        //实际屏幕宽高比
        float aspectRatio = Screen.width * 1.0f / Screen.height;

        float cameraWidth = orthographicSize * 2 * aspectRatio;

        

		CanvasScaler canvasScaler = (CanvasScaler)GameObject.Find("TextCaptureCanvas").GetComponent<CanvasScaler>();
        if (cameraWidth < devWidth)  //当前摄像机高度不够，导致宽度无法完整显示
        {
            orthographicSize = Mathf.Ceil(devWidth / (2 * aspectRatio));  //把高度调高
            canvasScaler.matchWidthOrHeight = 0;
        }
        else    //宽度够用，高度不够用
        {
            canvasScaler.matchWidthOrHeight = 1;
        }

        
        this.GetComponent<Camera>().orthographicSize = orthographicSize;

        rt.width = Screen.width;
        rt.height = Screen.height;


        if (Language.getInstance().language == Language.SimpleChinese)
        {
            googlePlayQRCodeObject.gameObject.SetActive(false);
            androidQRCodeObject.gameObject.SetActive(true);
        }
        else
        {
            googlePlayQRCodeObject.gameObject.SetActive(true);
            androidQRCodeObject.gameObject.SetActive(false);
        }

    }

    private Rect getCaptureMainRect() 
    {
        //实际屏幕宽高比
        float screenRatio = Screen.width * 1.0f / Screen.height;
        //设计宽高比
        float designRatio = devWidth / devHeight;

        float realWidth = 0f;
        float realHeight = 0f;

        float realCaptureWidth = 0f;
        float realCaptureHeight = 0f;

        float scaleRation = 1f; //缩放比

        Rect rect;

        
        

        if (designRatio < screenRatio) //相同高度下，屏幕要更宽；（宽度两边留白）
        {
            scaleRation = Screen.height / devHeight;   //高度缩放后与屏幕高度一致
            realWidth = scaleRation * devWidth;    //按比例缩放

            realCaptureWidth = scaleRation * captureWidth;
            realCaptureHeight = scaleRation * captureHeight;

            rect = new Rect((Screen.width - realWidth) / 2, 0, realCaptureWidth, realCaptureHeight);


            return rect;


        }
        else if (designRatio > screenRatio)       //相同高度下，设计宽度比屏幕宽，屏幕显示不下，最终缩放到宽度正好与屏幕一致（高度两边留白）
        {
            scaleRation = Screen.width / devWidth;
            realHeight = scaleRation * devHeight; //按比例缩放

            realCaptureWidth = scaleRation * captureWidth;
            realCaptureHeight = scaleRation * captureHeight;

            rect = new Rect(0, (Screen.height - realHeight) / 2, realCaptureWidth, realCaptureHeight);

            return rect;
        }

        //恰好相等 
        scaleRation = 1f;
        rect = new Rect(0, 0, captureWidth, captureHeight);


        return rect;
        
    }

    
    private Texture2D screenShot;
    public IEnumerator captureTextCoroutine(System.Action<Texture2D> _onCompletionHandler) 
    {

        if(Language.getInstance().language== Language.SimpleChinese)
        {
            googlePlayQRCodeObject.gameObject.SetActive(false);
            androidQRCodeObject.gameObject.SetActive(true);
        }
        else
        {
            googlePlayQRCodeObject.gameObject.SetActive(true);
            androidQRCodeObject.gameObject.SetActive(false);
        }
     

        this.gameObject.SetActive (true);
        if (null==screenShot) 
        {
            Rect rect = getCaptureMainRect();

            Camera cam = this.GetComponent<Camera>();

            cam.targetTexture = rt;

            cam.Render();

            //获取当前活动的RenderTexture
            RenderTexture currentActiveRT = RenderTexture.active;

            //将rt设置成当前活动的
            RenderTexture.active = rt;

            //Texture2D读取rt数据

			//DX与GL的坐标原点不一致，DX在左上角点，GL在左下脚点
            screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);

            screenShot.Apply();
            yield return new WaitForEndOfFrame();

            cam.targetTexture = null;
            RenderTexture.active = currentActiveRT;
        }
       

        //Fire the callback if exists
        if (_onCompletionHandler != null)
        {
            _onCompletionHandler(screenShot);
        }

		this.gameObject.SetActive (false);
		
    }

}
