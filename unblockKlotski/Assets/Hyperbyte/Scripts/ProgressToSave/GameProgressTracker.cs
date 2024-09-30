using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressTracker
{

    static ProgressData currentProgressData;

    /// <summary>
    /// 是否存在进度数据
    /// </summary>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public static bool HasGameProgress()
    {
        return PlayerPrefs.HasKey("gameProgress_currentgame");
    }


    public static void DeleteGameProgress()
    {
         PlayerPrefs.DeleteKey("gameProgress_currentgame");
    }

    /// <summary>
    /// 保存进度数据-序列化
    /// This method will be executed after each block shape being placed on board. This will get status of board, block shapes, timer, bombs, 
    /// score etc and will save to progress data class which inturn will be saved to playerprefs in json format.
    /// </summary>
    public static void SaveProgress()
    {
       
        if (BoosterManager.instance.isCanSave == false)
        {
            //当正在播放入场动画时不保存进度,因为会纪录正在飞的位置！！！
        }
        else
        {
            currentProgressData = new ProgressData();
            currentProgressData = PlayingManager.instance.progressDataToSave();
            string jsonToSave = JsonUtility.ToJson(currentProgressData);
            Debug.Log("jsonToSave:" + jsonToSave);
            //object转成json字符串，保存
            PlayerPrefs.SetString("gameProgress_currentgame", jsonToSave);
        }
    }

    /// <summary>
    /// Returns game progress for the given mode if any.
    /// </summary>
    public static ProgressData GetGameProgress()
    {
        if (HasGameProgress())
        {
            ProgressData progressData = JsonUtility.FromJson<ProgressData>(PlayerPrefs.GetString("gameProgress_currentgame"));
            if (progressData != null)
            {
                return progressData;
            }
        }
        return null;
    }
 
}

/// <summary>
/// Progress data class will be converted to json after preparing it to save game progress.
/// </summary>
[System.Serializable]
public class ProgressData
{
    public BlockContainerToSave blocksInGameToSave;
    public List<Vector3> listStartPosition;
    public int currentScore;
    public string GameMode;
    public int GameId;
    public int levelState;
    public int movestep;

    public ProgressData()
    {

    }
}
