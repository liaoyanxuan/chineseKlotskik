using Hyperbyte;
using sw.util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class iOSLeaderboardManager:ILeaderboardManager
{
    public float WAIT_SECOND = 10f;
    public initSetting initSetting;
  

	private static iOSLeaderboardManager instance = null;
	public static iOSLeaderboardManager getInstance()
	{
		if (instance == null)
		{
			instance=new iOSLeaderboardManager();
		}
		return instance;
	}

    private string StarNumID = "klotskistarnum";
    



    //检查上传状态，如果没有成功上传则上传；
    private iOSLeaderboardManager()
    {

    }

    private bool isAutheticateFail = false;
    //协程处理
    public void Authenticate(LeaderboarMode type) 
    {
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
                        Toast.instance.ShowMessage("排行榜登录失败");  //认证失败，无法上传分数
					}
					else if (LeaderboarMode.SHOW_BOARD == type)
					{
                        Toast.instance.ShowMessage("排行榜登录失败");
					}

					Debug.Log("Authenticationed Fail!");



                }
            });

        yield return new WaitForSeconds(WAIT_SECOND);  //等待10秒
        if (false == isAuthenticateRturn)   //无回应
        {   //弹出提示
         
            if (LeaderboarMode.SHOW_BOARD == type) //只有shwboard才提示
            {
				Toast.instance.ShowMessage("排行榜登录超时");  //认证失败，无法上传分数
            }

        }

    }


	
    private  void checkAndUploadScore() 
    {

        int starNumber = GameManager.instance.GetTotalStarEarnAllMode;
		reportScore(starNumber, StarNumID);

    }

	private void reportScore(long score, string leaderboardID)
	{
#if UNITY_IOS || GOOGLEPLAY ||!UNITY_EDITOR

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

		Social.ShowLeaderboardUI();
    }



}
