using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hyperbyte.Ads;
using Hyperbyte.Localization;
using sw.util;
using Hyperbyte;
using sw.game.evt;


[System.Serializable]
public class HintInfor
{
    public int blockIndex = 0;
    public int numberMoveRow = 0;
    public int numberMoveCol = 0;
}

[System.Serializable]
public class HintContainer
{
    public List<HintInfor> hints = new List<HintInfor>();
}
public class BoosterManager : MonoHandler
{
    public static BoosterManager instance;
    [SerializeField]
    private bool isMoveBeginGame = false;

    [SerializeField]
    private TextMeshProUGUI hintText;
    [SerializeField]
    private bool testMode;

    [SerializeField]
    private GameObject rankBtnObject;

    [SerializeField]
    private GameObject ageTipObject;

    [SerializeField]
    private GameObject adObject;

    [SerializeField]
    private RealNameVerifiedScript realNameVerifiedScript;


    private int maxWatchAds = 6;

    private int countAds = 0;

    private int hintNumber = 0;

    public bool isCanSave = false;

    public bool IsMoveBeginGame
    {
        get => isMoveBeginGame;

        set
        {
            // Debug.Log("BlockObj-->set isMoveBeginGame:" +value);
            isMoveBeginGame = value;
        }
    }

    private void Awake()
    {
        instance = this;
        countAds = 0;
        hintNumber = PlayerPrefs.GetInt("Hint", (testMode) ? 300 : 0);
        AddHint(0);
        /*
#if UNITY_ANDROID
#if GOOGLEPLAY
                rankBtnObject.SetActive(true);
#else
                rankBtnObject.SetActive(false);
#endif
#endif
        */
#if UNITY_IOS || GOOGLEPLAY
        ageTipObject.SetActive(false);
#else
        if (AndroidUtil.iSAgeTip == 1)
        {
            ageTipObject.SetActive(true);
        }
        else
        {
            ageTipObject.SetActive(false);
        }
        AndroidUtil.HideSplashScreen();


#endif

    }

    void Start()
    {

    }


    void showBanner()
    {
        Debug.Log("showBanner()-1");
        AdManager.Instance.ShowBanner();
    }


    public void OnEventHint()
    {
        if (hintNumber > 0)
        {
            AddHint(-1);
            PlayingManager.instance.HintGame();
          
        }
        else
        {

            UIController.Instance.toolUseQuery.Activate();
            UIController.Instance.toolUseQuery.GetComponent<ToolUseQueryScript>().SetToolReason(ToolUseQueryScript.USE_HINT);

            /*
            AdManagement.getInstance.UserChoseToWatchAd(AD_TYPE.HINT.ToString(), (b,s) =>
            {
                AddHint(1);
            });
            */
        }
    }

    public void AddHint(int number)
    {
      
        hintNumber += number;
        PlayerPrefs.SetInt("Hint", hintNumber);
        UpdateTextHint();
    }

    private void UpdateTextHint()
    {
        if(hintNumber==0)
        {
            hintText.text = "AD";
        }
        else
        {
            hintText.text = hintNumber.ToString() ;
        }
    }

    public void OnEventUndo()
    {
        if(countAds>=maxWatchAds)
        {
            /*
           if(GoogleMobileAdsScript.instance.CheckRewardBasedVideo())
            {
                GoogleMobileAdsScript.instance.ShowRewardBasedVideo();
            }
            */

            if (AdManager.Instance.IsRewardedAvailable())
            {
                AdManager.Instance.ShowRewardedWithTag("undo");
            }

            countAds = 0;
            return;
        }
        PlayingManager.instance.UndoGame();
    }

    public void OnEventReplay()
    {
        AdManager.Instance.IsInterstitialAvailable();
        PlayingManager.instance.ReplayGame();
    }

    public void OnRankShow()
    {
        LeaderboardManager.getInstance().Authenticate(LeaderboarMode.SHOW_BOARD);
    }


}
