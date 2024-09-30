using Hyperbyte;
using System.Collections;
using UnityEngine;


public class LeaderboardManager
{
    

	private static LeaderboardManager instance = null;
	public static LeaderboardManager getInstance()
	{
		if (instance == null)
		{
			instance=new LeaderboardManager();
		}
		return instance;
	}

    private ILeaderboardManager targetLeaderBoardManager;
    //检查上传状态，如果没有成功上传则上传；
    private LeaderboardManager()
    {

#if GOOGLEPLAY
        targetLeaderBoardManager = GooglPlayLeaderboardManager.getInstance();
#elif UNITY_IOS
      
        targetLeaderBoardManager=iOSLeaderboardManager.getInstance();
#else
        targetLeaderBoardManager = PlayFabLeaderboardManager.getInstance();
#endif
    }


    //协程处理
    public void Authenticate(LeaderboarMode type) 
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && type!= LeaderboarMode.UPLOAD_SCORE)
        {   //上传分数时不提示
            Toast.instance.ShowMessage("排行榜需要连接网络");
        }
        else
        {
            targetLeaderBoardManager.Authenticate(type);
        }
    }

}
