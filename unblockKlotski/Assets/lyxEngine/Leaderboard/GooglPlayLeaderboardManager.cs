using Hyperbyte;
using sw.util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GooglPlayLeaderboardManager:ILeaderboardManager
{
    public float WAIT_SECOND = 10f;
    public initSetting initSetting;
   

	private static GooglPlayLeaderboardManager instance = null;
	public static GooglPlayLeaderboardManager getInstance()
	{
		if (instance == null)
		{
			instance=new GooglPlayLeaderboardManager();
		}
		return instance;
	}

    private string StarNumID = "starnum";
    



    //检查上传状态，如果没有成功上传则上传；
    private GooglPlayLeaderboardManager()
    {
#if UNITY_ANDROID  && GOOGLEPLAY
        //无谷歌服务
        if (AndroidUtil.isGooglePlayServicesAvailable())
        {
            StarNumID = "CgkIxt665t4dEAIQAQ";
            GooglePlayGames.PlayGamesPlatform.Activate();
        }
#endif

    }

    private bool isAutheticateFail = false;
    //协程处理
    public void Authenticate(LeaderboarMode type) 
    {
#if UNITY_ANDROID
    #if GOOGLEPLAY
        //安卓无排行榜
        //无谷歌服务
        if (AndroidUtil.isGooglePlayServicesAvailable()==false)
        {
            return;
        }

        //登陆失败，不重试
        if (isAutheticateFail)
        {
            return;
        }
    #else
        return;
    #endif
#endif
       initSetting.Instance.StartCoroutine(AuthenticateCoroutine(type));

    }

    private IEnumerator AuthenticateCoroutine(LeaderboarMode type)
    {
            bool isAuthenticateRturn = false;   //是否调用有返回
		
			Social.localUser.Authenticate(success =>
			{
				if (success)
				{
                    isAuthenticateRturn=true;
					string userInfo = "Username: " + Social.localUser.userName
						+ " User ID: " + Social.localUser.id;

					Debug.Log("Authenticationessful");
					Debug.Log(userInfo);
				

					if (LeaderboarMode.UPLOAD_SCORE == type)
					{
						checkAndUploadScore();  //认证成功，检查并上传分数
					}
					else if (LeaderboarMode.SHOW_BOARD == type)
					{
                        checkAndUploadScore(); 
						ShowAllLeaderboardUI();
					}

				}
				else
				{
                    isAuthenticateRturn=true;
					if (LeaderboarMode.UPLOAD_SCORE == type)
					{
                        Toast.instance.ShowMessage(Language.getInstance().GetValue(Language.gameCenterOpenTips));  //认证失败，无法上传分数
					}
					else if (LeaderboarMode.SHOW_BOARD == type)
					{
                        Toast.instance.ShowMessage(Language.getInstance().GetValue(Language.gameCenterOpenTips));
					}

					Debug.Log("Authenticationed Fail!");

#if GOOGLEPLAY
                   
                    isAutheticateFail = true;
#endif


                }
            });

        yield return new WaitForSeconds(WAIT_SECOND);  //等待10秒
        if (false == isAuthenticateRturn)   //无回应
        {   //弹出提示
         
#if GOOGLEPLAY
            FlowingTipsScript.Instance.showTips("please login your Google Play Games account");
            isAutheticateFail = true;
#elif UNITY_IOS
            if (LeaderboarMode.SHOW_BOARD == type) //只有shwboard才提示
            {
                FlowingTipsScript.Instance.showTips(Language.getInstance().GetValue(Language.gameCenterOpenTips));  //认证失败，无法上传分数
            }
#endif
        }

    }



    private  void checkAndUploadScore() 
    {

        int starNumber = PlayerPrefs.GetInt(string.Format("TotalStar_{0}", 0), 0);
        reportScore(starNumber, StarNumID);

    }

	private void reportScore(long score, string leaderboardID)
	{
#if UNITY_IOS || GOOGLEPLAY

        Debug.Log("Reported score");
			Social.ReportScore(score, leaderboardID, success => {
				Debug.Log(success ? 
					"Reported score successfully" 
					: "Failed to report score");
				if(success)
				{
                   
                   
                }
                else
				{
					
				}
			});
#endif
    }

	private void ShowAllLeaderboardUI()
	{
#if UNITY_IOS || GOOGLEPLAY
		Social.ShowLeaderboardUI();
#else
         UIController.Instance.leaderBoardLogin.Activate();
#endif
    }



}
