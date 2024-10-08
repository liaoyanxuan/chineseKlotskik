#if GOOGLEPLAY || UNITY_IOS
using GoogleMobileAds.Ump.Api;
#endif

using Hyperbyte.Ads;

using sw.util;

using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class InitConfig : MonoBehaviour
{
    private static InitConfig _instance;
    //单例模式
    public static InitConfig Instance
    {
        get
        {
            if (null != _instance)
            {
                return _instance;
            }
            InitConfig[] _monoComponents = FindObjectsOfType(typeof(InitConfig)) as InitConfig[];
            _instance = _monoComponents[0];
            return _instance;
        }

    }

    [SerializeField]
    private GameObject adObject;




    private bool isGDPR_Verified = false; // GDPR完成
    private bool isIDFA_Verified = false; // IDFA完成

    public string testUrl = "https://www.microsoft.com"; // 测试网络连接的 URL


    void Awake()
    {
        DebugEx.LogImportant("testload  InitConfig start tm:" + Time.time);
        _instance = this;

    }

    void Start()
    {



        StartCoroutine(ShowPopups());

    }

    IEnumerator ShowPopups()
    {

        DebugEx.LogImportant("testload  ShowPopups:" + Time.time);

 
        //GDPR弹窗
        TimerSchedule.Schedule(this, 1f, starGDPRandPopReview);
        yield return new WaitUntil(() => isGDPR_Verified); //等待 GDPR完成

        DebugEx.LogImportant("testload  GDPR弹窗完成:" + Time.time);

        yield return new WaitForSeconds(0.5f);

        idfaRequest();  //idfa弹窗
        yield return new WaitUntil(() => isIDFA_Verified); //等待 IDFA完成

        DebugEx.LogImportant("testload IDFA弹窗完成:" + Time.time);

        initADs();
    }






    private void starGDPRandPopReview()
    {

        if (iOSUtil.isNeedGDPR())
        {
#if UNITY_IOS || GOOGLEPLAY
            if (iOSUtil.IsDebugTest == 1 || AndroidUtil.IsDebugTest == 1)
            {
                DebugEx.LogImportant("isNeedGDPR-ConsentInformation.Reset!");
                ConsentInformation.Reset();
            }

            //处理欧盟GDPR
            GDPRRequest.Instance.startCheck(gdprCallback);
#endif
        }
        else
        {
            gdprCallback("notNeedGdpr");
        }
    }

    private bool needShowIdfa = true;
    private void gdprCallback(string result)
    {
        DebugEx.LogImportant("gdprCallback:" + result);
        isGDPR_Verified = true;  //GDPR完成

        // Example value: "1111111111"
        string purposeConsents = ApplicationPreferences.GetString("IABTCF_PurposeConsents");
        // Purposes are zero-indexed. Index 0 contains information about Purpose 1.
        DebugEx.LogImportant("purposeConsents", purposeConsents);
        if (!string.IsNullOrEmpty(purposeConsents))
        {
            char purposeOneString = purposeConsents[0];
            bool hasConsentForPurposeOne = purposeOneString == '1';

            // 使用 IndexOf() 检查是否包含 '1'
            needShowIdfa = purposeConsents.IndexOf('1') >=0;  //gdpr询问中全部为零，不需要弹idfa
            DebugEx.LogImportant("purposeConsents-needShowIdfa", needShowIdfa);
        }

    }

    private void idfaRequest()
    {
        int gameStart = PlayerPrefs.GetInt("GameStartCount", 0);
        gameStart = gameStart + 1;
        PlayerPrefs.SetInt("GameStartCount", gameStart);

        //只会弹一次；且idfa弹窗不全为零
        if (gameStart == 1 && needShowIdfa)
        {
            iOSUtil.requestIDFA(requestIDFAResult); //iOS等待回掉
        }
        else
        {
            requestIDFAResult("0"); //idfa已经处理过
        }

        if (gameStart >= 3)
        {
            ShowReviewPopup();
        }
    }

    //idfa回调
    private void requestIDFAResult(string result)
    {
        isIDFA_Verified = true;
        DebugEx.LogImportant("requestIDFAResult " + result);


    }


    void initADs()
    {
       
        adObject.SetActive(true);
        TimerSchedule.Schedule(this,10f,showBanner);

    }



    void showBanner()
    {
        DebugEx.LogImportant("showBanner()-1");
        AdManager.Instance.ShowBanner();
    }


    private void ShowReviewPopup()
    {

#if UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#endif

    }



}
