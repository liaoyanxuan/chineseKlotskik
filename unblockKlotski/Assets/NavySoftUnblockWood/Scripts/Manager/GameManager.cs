//help@navysoftgames.com version 1.1 at unityasset
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PopUpFrameWork;
using Hyperbyte.Ads;
using Hyperbyte.Localization;
using sw.util;

[System.Serializable]
public class GridContainer
{
    public List<GameObject> grids = new List<GameObject>();
}
[System.Serializable]
public class GameModeInfor
{
    public string nameGameMode;
    public GAME_MODE_ID idGameMode;
    public Sprite bannerGameMode;
    public int GAME_MODE_STAR = 1;
    public int starRequest = 1;
    public int totalStarInMode = 0;
    public int currentLevel;

 
 
    public List<TextAsset> datas = new List<TextAsset>();
 

    public void SaveLevel()
    {
      
        PlayerPrefs.SetInt(string.Format("Mode_{0}",idGameMode),currentLevel);
    }
    public void LoadLevel()
    {
       currentLevel = PlayerPrefs.GetInt(string.Format("Mode_{0}", idGameMode), 1);
     
    }

    

    public void SaveTotalStar()
    {
        PlayerPrefs.SetInt(string.Format("TotalStar_{0}", idGameMode), totalStarInMode);
    }


    public void LoadTotalStar()
    {
        totalStarInMode = PlayerPrefs.GetInt(string.Format("TotalStar_{0}", idGameMode), 0);
    }


}


public enum GAME_MODE_ID
{
    CLASSIC = 0,
    BLINDBOX = 1,
    LEVEL_1 = 10,
    LEVEL_2 = 2,
    LEVEL_3 = 20,
    LEVEL_4 = 3,
    LEVEL_5 = 30,
    LEVEL_6 = 4,
    LEVEL_7 = 40,
}


[System.Serializable]
public class Level
{
    public int levelStage;
    public int levelShowStage;
    public GAME_MODE_ID gameMode;

    
    [HideInInspector]
    public int valueAddTotalStar;

    [HideInInspector]
    public string data;
   

    public LevelData levelData = new LevelData();
    public HintContainer hintContainer = new HintContainer();


    public void Load()
    {
        
        LoadBlock();
    }
  
    private void LoadBlock()
    {
        if (levelData.blockInfors.Count > 0) return;
        BlockInfor[] blocksInfor = JsonHelper.FromJson<BlockInfor>(data);

       
        for (int i = 0; i < blocksInfor.Length; i++)
        {
            levelData.blockInfors.Add(blocksInfor[i]);
        }

        
    }
    public void SaveStar(int starEarn)
    {
        int star = GetStar();
        valueAddTotalStar = 0;
        if (starEarn > star)
        {
            valueAddTotalStar = starEarn - star;
           
            PlayerPrefs.SetInt(string.Format("Star_{0}_{1}", gameMode, levelStage), starEarn);
        }

    }
    public int GetStar()
    {
       return  PlayerPrefs.GetInt(string.Format("Star_{0}_{1}", gameMode, levelStage), 0);
    }


    public void SaveBestMove(int bestMove)
    {
        PlayerPrefs.SetInt(string.Format("Move_{0}_{1}", gameMode, levelStage), bestMove);
    }
    public int LoadBestMove()
    {
       return  PlayerPrefs.GetInt(string.Format("Move_{0}_{1}", gameMode, levelStage), 0);
    }

    public void SetUnlock()
    {
        PlayerPrefs.SetInt(string.Format("Unlock_{0}_{1}", gameMode, levelStage), 1);
    }
    public int GetUnlock()
    {
        return PlayerPrefs.GetInt(string.Format("Unlock_{0}_{1}", gameMode, levelStage), 0);
    }


}
public class GameManager : MonoBehaviour
{
    //还原进度,恢复游戏
    public bool isResumeGame = false;

    public static string GAME_MODE;
    public const int gridWidth = 6;
    public const int gridHeight = 6;
    public const float  speedMoveBlock = 0.5f;
    public const int totalLevelInPage = 9;
    public static GameManager instance;
    public const int maxWatchInterstitial = 3;

    public int interAdWatchTime = 0;
    public const int INTER_AD_WATCH_REWARD = 3; //5次插屏奖励

    [SerializeField]
    private List<GameModeInfor> gameModeInfors = new List<GameModeInfor>();

    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI moveText;
    [SerializeField]
    private TextMeshProUGUI bestMoveText;

    [SerializeField]
    private TextMeshProUGUI blockTitleText;


    

    public Dictionary<GAME_MODE_ID, List<Level>> AllLevels = new Dictionary<GAME_MODE_ID, List<Level>>();
    public int move;
    private int bestMove;
    private int totalStarEarnAllMode;
    private int totalStarAllMode=2934;
  
    /*
        gameModeStar-->:CLASSIC-->:340
        gameModeStar-->:BLINDBOX-->:276
        gameModeStar-->:LEVEL_1-->:46
        gameModeStar-->:LEVEL_2-->:206
        gameModeStar-->:LEVEL_3-->:313
        gameModeStar-->:LEVEL_4-->:446
        gameModeStar-->:LEVEL_5-->:471
        gameModeStar-->:LEVEL_6-->:516
        gameModeStar-->:LEVEL_7-->:320
     */

    public DataInGame DataInGame { get; set; }

    public List<GameModeInfor> GetAllGameMode { get { return gameModeInfors; } }
    public Level CurrentLevel;//{ get; private set; }
    public GameModeInfor CurrentGameMode { get; set; }
    public GAME_MODE_ID levelGameMode;//{ get; private set; }
    public List<Level> CurrentLevelsInGameMode;//{ get; private set; }
    [SerializeField]
    private bool testMode = false;
    public int GetTotalStarEarnAllMode { get { return totalStarEarnAllMode; } }




    private void Awake()
    {
        instance = this;
       
        int totalStage = 0;
        for (int i = 0; i < gameModeInfors.Count; i++)
        {
            GameModeInfor game = gameModeInfors[i];
          
            game.LoadTotalStar();
         
            totalStarEarnAllMode += game.totalStarInMode;

            AllLevels.Add(game.idGameMode, new List<Level>());

            if(i>0)
            {
                totalStage += gameModeInfors[i-1].datas.Count;
                
            }

            game.LoadLevel();
           /****
             int gameModeStar = 0;
           */

          for (int j = 0; j < game.datas.Count; j++)
          {
              Level level = new Level();
              level.data = game.datas[j].text;
              level.levelStage =  j + 1;
              level.levelShowStage = totalStage+ j + 1;   //显示第几关
              level.gameMode = game.idGameMode;
              AllLevels[game.idGameMode].Add(level);


              //这里计算一下总星星数，预先计算不实时计算
              /****
              level.Load();
              int levelStart= (int)Mathf.Ceil(level.levelData.blockInfors[0].mini / 10f);
              gameModeStar += levelStart;
              */
        }

        /****
           Debug.Log("gameModeStar-->:"+ game.idGameMode +"-->:"+ gameModeStar);
           totalStarAllMode += gameModeStar;

            Debug.Log("totalStarAllMode-->:"+ totalStarAllMode);
         */

        }

//#if UNITY_EDITOR
        if (testMode)
        {
            totalStarEarnAllMode = 1000;
        }
//        #endif

        DataInGame = new DataInGame();
        DataInGame = DataInGame.Load();


        LoadInterAdWatchTime();

       
    }


    public void SaveInterAdWatchTime()
    {
        PlayerPrefs.SetInt("InterAdWatchTime", interAdWatchTime);
    }

    public void LoadInterAdWatchTime()
    {
        interAdWatchTime = PlayerPrefs.GetInt("InterAdWatchTime", 0);
    }


    private int CaculatorStarLevel()
    {
        int limitMove = CurrentLevel.levelData.blockInfors[0].mini;

        float totalStar=Mathf.Ceil(limitMove / 10f); //总星星

        int exceeded = move - limitMove; //超出最佳步数
        float minusStar=Mathf.Ceil(exceeded / 10f);  //需要扣除星星

        float finalStar = Mathf.Max(1,totalStar - minusStar); //最终星星数,至少1颗

        Debug.Log("CaculatorStarLevel:move:" + move + "-->limitMove:" + limitMove + " totalStar:" + totalStar);
        Debug.Log("CaculatorStarLevel:exceeded:" + exceeded + "-->minusStar:" + minusStar + " finalStar:" + finalStar);

        return (int)finalStar;

    }

    public void SelectedGameMode(GAME_MODE_ID gameModeId)
    {
        for (int position = 0; position < gameModeInfors.Count; position++)
        {
            if (gameModeInfors[position].idGameMode == gameModeId)
            {
                CurrentGameMode = gameModeInfors[position];
            }
        }

        levelGameMode = gameModeId;
        CurrentLevelsInGameMode = AllLevels[gameModeId];
    }

    public List<Level> GetListLevelPage(int position)
    {
        List<Level> levels = new List<Level>();
        int start = position * totalLevelInPage;
        int end = (position + 1) * totalLevelInPage;
        for (int i = start; i < end; i++)
        {
            if (i >= CurrentLevelsInGameMode.Count) break;
            levels.Add(CurrentLevelsInGameMode[i]);
        }
        return levels;
    }

    public int GetTotalPageMode()
    {
        float total = CurrentLevelsInGameMode.Count;
        return Mathf.CeilToInt(total / totalLevelInPage);
    }
    public string TotalStarInGameMode(GAME_MODE_ID gameModeId)
    {
        GameModeInfor gameModeInfor=null;

        for (int position = 0; position < gameModeInfors.Count; position++)
        {
            if (gameModeInfors[position].idGameMode == gameModeId)
            {
                gameModeInfor = gameModeInfors[position];
            }
        }


        int totalStarMode = gameModeInfor.GAME_MODE_STAR;

        return string.Format("{0}/{1}", gameModeInfor.totalStarInMode, totalStarMode);

    }

    public string TotalStarInAllMode()
    {

        return string.Format("{0}/{1}", totalStarEarnAllMode, totalStarAllMode);

    }



    public void PlayGame(int movestep=0)
    {
        winReplayCurrentLevel = null;
        levelText.text = string.Format("{0}", CurrentLevel.levelShowStage);
        move = movestep;
        moveText.text = string.Format("{0}", move);
        blockTitleText.text = CurrentLevel.levelData.blockInfors[0].blockname; //关卡名
        bestMove = CurrentLevel.LoadBestMove();
        bestMoveText.text = string.Format("{0}/{1}", (bestMove==0?"--":bestMove.ToString()), bestMove == 0 ? "?" : CurrentLevel.levelData.blockInfors[0].mini.ToString());
    }

    public void RePlayGame(int movestep = 0)
    {
        if (winReplayCurrentLevel != null)
        {
            CurrentLevel = winReplayCurrentLevel;
            winReplayCurrentLevel = null;
        }
       
        move = movestep;
        moveText.text = string.Format("{0}", move);
        blockTitleText.text = CurrentLevel.levelData.blockInfors[0].blockname; //关卡名
        bestMove = CurrentLevel.LoadBestMove();
        bestMoveText.text = string.Format("{0}/{1}", (bestMove == 0 ? "--" : bestMove.ToString()), bestMove == 0 ? "?" : CurrentLevel.levelData.blockInfors[0].mini.ToString());
    }

    public void UpdateMove(bool undo)
    {
        if (undo)
        {
            move--;     //新逻辑
        }
        else
        {
            move++;  //原逻辑
        }
       
      
        moveText.text = string.Format("{0}", move);
    }


    public void SetBestMove()
    {
        if(bestMove==0)
        {
            bestMove = move;
            
           
            CurrentLevel.SaveBestMove(bestMove);
        }
        else if (bestMove > move)
        {
            bestMove = move;
            CurrentLevel.SaveBestMove(bestMove);
        }


    }
    public List<Level> GetLevelGameMode(GAME_MODE_ID gameModeId)
    {
        return AllLevels[gameModeId];

    }

    public void NextLevel()
    {    
        CurrentLevel = getNextLevel();
        CurrentLevel.Load();
        PlayGame();
        PlayingManager.instance.PlayGame();
    }

    public Level getNextLevel()
    {

        GAME_MODE_ID gameMode2 = CurrentLevel.gameMode;
        int position = 0;
        for (position = 0; position < gameModeInfors.Count; position++)
        {
            if (gameModeInfors[position].idGameMode == gameMode2)
            {
                break;
            }
        }
        GameModeInfor currentGameModeInfor = gameModeInfors[position];
        //按顺序未通关关卡
        int newCurrentLevel = currentGameModeInfor.currentLevel;
        CurrentLevel = AllLevels[gameMode2][newCurrentLevel-1];

        int nextLevel = CurrentLevel.levelStage - 1;


        GAME_MODE_ID gameMode = CurrentLevel.gameMode;

        if (nextLevel >= AllLevels[gameMode].Count)
        {
            nextLevel = 0;
            int positon = 0;
            for (positon = 0; positon < gameModeInfors.Count; positon++)
            {
                if (gameModeInfors[positon].idGameMode == gameMode)
                {
                    break;
                }
            }

            positon = positon + 1;
            if (positon < gameModeInfors.Count)
            {
                gameMode = gameModeInfors[positon].idGameMode;
            }
            else
            {
                gameMode = 0;
            }
        }

        Level theNextLevel = AllLevels[gameMode][nextLevel];

        return theNextLevel;
    }


#if UNITY_IOS || GOOGLEPLAY
    private int countInterstitial = 0;
#else
    private int countInterstitial = 0;
#endif

    public Level winReplayCurrentLevel;
    System.Object[] datasSend;
    public void WinGame()
    {
        SetBestMove();
       
        int star = CaculatorStarLevel();

        winReplayCurrentLevel = CurrentLevel;

        CurrentLevel.SaveStar(star);

        GAME_MODE_ID gameMode2 = CurrentLevel.gameMode;

        int position = 0;

        for (position = 0; position < gameModeInfors.Count; position++)
        {
            if (gameModeInfors[position].idGameMode == gameMode2)
            {
                break;
            }
        }

        gameModeInfors[position].totalStarInMode += CurrentLevel.valueAddTotalStar;
        totalStarEarnAllMode+= CurrentLevel.valueAddTotalStar;
        gameModeInfors[position].SaveTotalStar();


        GameModeInfor currentGameModeInfor = gameModeInfors[position];

        bool isLastGuanka = false;
        if (winReplayCurrentLevel.levelStage >= currentGameModeInfor.datas.Count)
        {
            isLastGuanka = true;
        }

        //按顺序未通关关卡
        int newCurrentLevel = currentGameModeInfor.currentLevel;
        CurrentLevel = AllLevels[gameMode2][newCurrentLevel-1];
      
        //当前关卡为此级别中按顺序走的第一个未通关关卡或最后一关
        while (CurrentLevel.GetStar() > 0)  //已通关
        {
            newCurrentLevel++;   //加一关
            if (newCurrentLevel > currentGameModeInfor.datas.Count) //最后一关
            {
                newCurrentLevel = currentGameModeInfor.datas.Count;
               
                break;
            }
            else
            {
                CurrentLevel = AllLevels[gameMode2][newCurrentLevel-1];
               
            }
        }

        CurrentLevel.Load();
        if (CurrentLevel.GetStar() == 0) 
        {
            isLastGuanka = false;
        }
      

        if (gameModeInfors[position].currentLevel != newCurrentLevel)
        {
            gameModeInfors[position].currentLevel = newCurrentLevel;
            gameModeInfors[position].SaveLevel();
        }

        datasSend = new System.Object[4];
        datasSend[0] = star;
        datasSend[1] = move;
        datasSend[2] = bestMove;
        datasSend[3] = isLastGuanka;

        showPopWin();
        AdManager.Instance.IsInterstitialAvailable();

        countInterstitial++;
        if (countInterstitial >= GameManager.maxWatchInterstitial)
        {   //达到播放条件
            if (AdManager.Instance.IsInterstitialAvailable())
            {
                countInterstitial = 0;
                StartCoroutine(ShowADTips());
            }
        }

    }


    private void showPopWin()
    {
      //  SoundManager.instance.WinGameSound();
        PopupManager.instance.Show("win", datasSend);
    }

    bool isReward = false;
    private IEnumerator ShowADTips()
    {
        yield return new WaitForSeconds(0.2f);

        isReward = false;

        interAdWatchTime++;   //增加插屏观看次数
        if (interAdWatchTime > INTER_AD_WATCH_REWARD)
        {
            interAdWatchTime = INTER_AD_WATCH_REWARD;
        }

        PopupADTips popupAdTips = (PopupADTips)PopupManager.instance.GetPopupById("adtips");
        popupAdTips.setProcessTxt(interAdWatchTime+"/"+ INTER_AD_WATCH_REWARD);

        PopupManager.instance.Show("adtips");//插屏提示
        yield return new WaitForSeconds(1.5f);

        //////////////////////////////////////////////////////////
        ///
        AdManager.Instance.ShowInterstitial((isClose)=>
        {
            if (isReward)
            {
                Debug.Log("interAd Close Reward!!");
            }
            else
            {
                Debug.Log("interAd Close not Reward");
            }


        });  //播放广告；

        yield return new WaitForSeconds(1.5f);

        popupAdTips.Hide(true);


        if (interAdWatchTime >= INTER_AD_WATCH_REWARD)
        {
            isReward = true;
            BoosterManager.instance.AddHint(1);
            interAdWatchTime = 0;
        }

        SaveInterAdWatchTime();//保存插屏观看次数

    }


    private void popADThank()
    {
        //感谢观看/奖励动画
        PopupADThanks popupAdThanks = (PopupADThanks)PopupManager.instance.GetPopupById("adthanks");
        popupAdThanks.setProcessTxt(interAdWatchTime + "/" + INTER_AD_WATCH_REWARD);

        if (interAdWatchTime >= INTER_AD_WATCH_REWARD)
        {
          
            popupAdThanks.setThanksTxt(LocalizationManager.Instance.GetTextWithTag("ADThanksReward"));
        }
        else
        {
            popupAdThanks.setThanksTxt(LocalizationManager.Instance.GetTextWithTag("ADThanksContent"));
        }

        PopupManager.instance.Show("adthanks");//插屏感谢 (如果获得奖励，感谢观看，已获得奖励)
    }


    public int GetLevelCompleteMode(GAME_MODE_ID gameModeId)
    {
        GameModeInfor gameModeInfor = null;
        for (int position = 0; position < gameModeInfors.Count; position++)
        {
            if (gameModeInfors[position].idGameMode == gameModeId)
            {
                gameModeInfor = gameModeInfors[position];
            }
        }


        int levelComplete = gameModeInfor.currentLevel;

//#if UNITY_EDITOR
        if (testMode)
        {
            return 1000;
        }
//#endif
        return levelComplete;
    }
   
}
