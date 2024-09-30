using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LeaderboarMode
{
    UPLOAD_SCORE,  //上传分数
    SHOW_BOARD  //显示排行榜
}

interface ILeaderboardManager 
{
    void Authenticate(LeaderboarMode type);

}
