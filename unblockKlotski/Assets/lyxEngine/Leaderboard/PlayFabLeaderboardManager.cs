using Hyperbyte;
using LeaderBoardSuperScrollView;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabLeaderboardManager:ILeaderboardManager
{
    public float WAIT_SECOND = 10f;
    public initSetting initSetting;
   
	private static PlayFabLeaderboardManager instance = null;
	public static PlayFabLeaderboardManager getInstance()
	{
		if (instance == null)
		{
			instance=new PlayFabLeaderboardManager();
		}
		return instance;
	}

    private string StarNumID = "total_score";
    



    //检查上传状态，如果没有成功上传则上传；
    private PlayFabLeaderboardManager()
    {
        PlayFabAuthServiceForManager.OnLoginSuccess += OnPlayFabLoginSuccess;
        PlayFabAuthServiceForManager.OnPlayFabError += OnPlayFabLoginError;

    }

    private void OnPlayFabLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        UIController.Instance.HideLoadingTip();
        string diaplayName = null;
        if (result.InfoResultPayload.AccountInfo.TitleInfo != null)
        {
            diaplayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
        }

        PlayFabAuthService.Instance.playFabId = result.PlayFabId;

        if (diaplayName == null)
        {
            //显示登陆
            UIController.Instance.leaderBoardLogin.Activate();
        }
        else
        {
            PlayFabAuthService.Instance.tempDisplayNameId = diaplayName;
     

            Authenticate(_currentType);
        }

       
    }


    //协程处理
    public void Authenticate(LeaderboarMode type) 
    {
       
            initSetting.Instance.StartCoroutine(AuthenticateCoroutine(type));
     
    }

    private LeaderboarMode _currentType= LeaderboarMode.UPLOAD_SCORE;
    private IEnumerator AuthenticateCoroutine(LeaderboarMode type)
    {
        _currentType = type;
        //本地已登陆有昵称，假设seesion未过期
        if (string.IsNullOrEmpty(PlayFabAuthService.Instance.tempDisplayNameId) ==false)
        {
           
            if (LeaderboarMode.UPLOAD_SCORE == type)
            {
                checkAndUploadScore();  //认证成功，检查并上传分数
            }
            else if (LeaderboarMode.SHOW_BOARD == type)
            {
                checkAndUploadScore(true);
               
            }
        }
        else
        {
            //已记录过昵称但本次未登陆
            if (string.IsNullOrEmpty(PlayFabAuthServiceForManager.Instance.RememberMeDisplayNameId) == false)
            {
                PlayFabAuthServiceForManager.Instance.InfoRequestParams = new GetPlayerCombinedInfoRequestParams{ GetUserAccountInfo =true};
                //静默登陆

                PlayFabAuthServiceForManager.Instance.Authenticate(Authtypes.Silent);
                if(_currentType!= LeaderboarMode.UPLOAD_SCORE)
                {
                    UIController.Instance.ShowLoadingTip();
                }
               
            }
            else
            {
                //未记录过昵称
                UIController.Instance.leaderBoardLogin.Activate();
            }

        }


        yield return null;

    }



    private  void checkAndUploadScore(bool showBoard=false) 
    {

        int starNumber = GameManager.instance.GetTotalStarEarnAllMode;
        SubmitScore(starNumber, StarNumID, showBoard);

    }


    private int _uploadedSocre = -1;
    public void SubmitScore(int playerScore, string leaderboardID,bool showBoard = false)
    {
        if (_uploadedSocre == playerScore)  //分数已上传过
        {
            if (showBoard)
            {
                ShowAllLeaderboardUI();
            }
            return;
        }


        if (showBoard)
        {
            Debug.Log("SubmitScore");
            UIController.Instance.ShowLoadingTip();
        }

        CustomerData customerData = new CustomerData { uploadedScore= playerScore,isShowBoard=showBoard };

        //展示loading
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = leaderboardID,
                    Value = playerScore
                }
            }
        }, result => OnStatisticsUpdated(result), FailureCallback, customerData);
    }



    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {

        CustomerData customerData = (CustomerData)updateResult.CustomData;
        _uploadedSocre = customerData.uploadedScore;
        Debug.Log("Successfully submitted high score"+ customerData.isShowBoard.ToString());

        
        if (customerData.isShowBoard)
        {
            if (PlayFabAuthService.Instance.firstDisplay)
            {
                TimerSchedule.Schedule(initSetting.Instance, 2f, ShowAllLeaderboardUI);
            }
            else
            {
                ShowAllLeaderboardUI();
            }

        }
     
    }



    private int MAX_RESULT = 11;
    //Get the players with the top 10 high scores in the game
    public void ShowAllLeaderboardUI()
    {
        UIController.Instance.ShowLoadingTip();
        RequestMeRank();
        //展示loading
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = StarNumID,
            StartPosition = 0,
            MaxResultsCount = MAX_RESULT
        }, result => DisplayLeaderboard(result), FailureCallback);

        
    }

    private void DisplayLeaderboard(GetLeaderboardResult result)
    {
        UIController.Instance.HideLoadingTip();

        LeaderBoardDataSourceMgr.Get.ClearItem();
        foreach (PlayerLeaderboardEntry item in result.Leaderboard)
        {
           // Debug.Log(string.Format("PLACE:{0}|NAME:{1}|VALUE:{2}|VALUE:{3}", item.Position, item.DisplayName, item.StatValue, item.PlayFabId));
            LeaderBoardDataSourceMgr.Get.InsertSourceData(item.Position, item.DisplayName, item.StatValue, item.PlayFabId);

            //item.PlayFabId; //判断是否是自己
        }

        UIController.Instance.gameLeaderBoard.Activate();

        UIController.Instance.gameLeaderBoard.GetComponent<LeaderBoardPullToLoadMoreDemoScript>().ShowBoardData();
    }

    //请求自己的排行榜
    public void RequestMeRank()
    {
        GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = StarNumID,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, FailureCallback);
    }

    private void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult result)
    {

        PlayerLeaderboardEntry item = result.Leaderboard[0];
        
       // Debug.Log(string.Format("MY-PLACE:{0}|NAME:{1}|VALUE:{2}|VALUE:{3}", item.Position, item.DisplayName, item.StatValue, item.PlayFabId)); //item.PlayFabId

        UIController.Instance.gameLeaderBoard.GetComponent<GameLeaderBoardScript>().SetMyRankData(item.Position, item.DisplayName, item.StatValue);

     
    }

    System.Action mOnLoadMoreFinished = null;
    public void RequestLoadMoreDataList(System.Action onLoadMoreFinished)
    {
        mOnLoadMoreFinished = onLoadMoreFinished;
        int starPos=LeaderBoardDataSourceMgr.Get.TotalItemCount;

        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = StarNumID,
            StartPosition = starPos,
            MaxResultsCount = MAX_RESULT
        }, result => DisplayLeaderboardMore(result), FailureCallback);
    }

    private void DisplayLeaderboardMore(GetLeaderboardResult result)
    {
        if (result.Leaderboard.Count == 0)
        {
            Toast.instance.ShowMessage("已到最后");
        }
        foreach (PlayerLeaderboardEntry item in result.Leaderboard)
        {
            Debug.Log(string.Format("PLACE:{0}|NAME:{1}|VALUE:{2}", item.Position, item.DisplayName, item.StatValue));
            LeaderBoardDataSourceMgr.Get.InsertSourceData(item.Position, item.DisplayName, item.StatValue, item.PlayFabId);
        }

        if (mOnLoadMoreFinished != null)
        {
            mOnLoadMoreFinished();
        }
    }

    private void FailureCallback(PlayFabError error)
    {

        UIController.Instance.HideLoadingTip();
        //There are more cases which can be caught, below are some
        //of the basic ones.
        switch (error.Error)
        {   //session过期
            case PlayFabErrorCode.InvalidSessionId:
            case PlayFabErrorCode.InvalidSessionTicket:
            case PlayFabErrorCode.RegistrationSessionNotFound:
            case PlayFabErrorCode.SessionLogNotFound:
                PlayFabAuthService.Instance.tempDisplayNameId = null;
                PlayFabAuthService.Instance.playFabId = null;
                PlayFabAuthServiceForManager.Instance.Authenticate(Authtypes.Silent);
                break;
            case PlayFabErrorCode.AccountNotFound:
                return;
            default:
                break;
        }
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:"+ error.Error.ToString());
        Debug.LogError(error.GenerateErrorReport() + " " + error.Error);
    }



    private void OnPlayFabLoginError(PlayFabError error)
    {
        PlayFabAuthService.Instance.tempDisplayNameId = null;
        PlayFabAuthService.Instance.playFabId = null;
        UIController.Instance.HideLoadingTip();
        UIController.Instance.ShowAlertTip("排行榜登陆失败：" + error.GenerateErrorReport() + " " + error.Error);
        Debug.LogError("登陆失败:"+" "+ error.Error);


       // error.Error==ServiceUnavailable   断网状态下，服务器不可用
    }

    class CustomerData
    {
        public int uploadedScore = -1;
        public bool isShowBoard = false;
    }

}
