using System.Collections;
using System.Collections.Generic;
using System.Text;
using Hyperbyte;
using Hyperbyte.Ads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class UndoAction
{
    public GameObject block;
    public BlockObj currentBlockObj;
    public Vector3 prevPosition;

    public void Action()
    {
        block.transform.localPosition = prevPosition;
        currentBlockObj.savePosition = prevPosition;
    }
}

public enum HEROID
{
    GUANYU=1,
    ZHANGFEI=2,
    ZHOYUN=3,
    MACHAO=4,
    HUANGZHONG=5
}

public class PlayingManager : MonoHandler
{
    public static PlayingManager instance;
    [SerializeField]
    private List<GridContainer> gridInGame = new List<GridContainer>();
    [SerializeField]
    private List<GameObject> blocksPrefab = new List<GameObject>();
    [SerializeField]
    private Transform blockContainer;
    [SerializeField]
    private GameObject arrowImage;

    [SerializeField]
    private List<BlockObj> blockObjsInGame = new List<BlockObj>();

    [SerializeField]
    private List<Button> buttonsInGame = new List<Button>();

    [SerializeField]
    private List<Sprite> verticalBlock = new List<Sprite>();


    [SerializeField]
    private TextMeshProUGUI hintStepText;

    [SerializeField]
    private Button preHintBtn;

    [SerializeField]
    private Button nextHintBtn;

    [SerializeField]
    private GameObject hintContent;

    private Vector2 FirstSelectPoint;
    private Vector2 FirstBlockPoint;
    private Vector3 selectedPositionBlock = Vector3.zero;
    private List<Vector3>  listStartPosition = new List<Vector3>();
    private List<UndoAction> undoActions = new List<UndoAction>();
  
    private BlockObj blockSpecial;
  
    [Header("Debug ")]
    [SerializeField] private GameObject selectedBlock;
    [SerializeField] private BlockObj shadowBlock;
  
    [SerializeField]
    private int blockMovePointMax;
    [SerializeField]
    private int blockMovePointMin;

    [SerializeField] private List<BlockObj> blockObjsWidth1 = new List<BlockObj>();

    [SerializeField] private List<BlockObj> blockObjsWidth2a = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsWidth2b = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsWidth2c = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsWidth2d = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsWidth2e = new List<BlockObj>();


    [SerializeField] private List<BlockObj> blockObjsHeight2a = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsHeight2b = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsHeight2c = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsHeight2d = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsHeight2e = new List<BlockObj>();
    [SerializeField] private List<BlockObj> blockObjsHeight3 = new List<BlockObj>();


    [SerializeField] private List<BlockObj> blockObjSpeicals = new List<BlockObj>();

    private bool blockDrag = false;
    private Vector3 originPositionBlock;
    private int countHint = 0;
    private bool isHint = false;
    private bool isPause = false;
    private float startDrag = 0;
    private float endDrag =  0;
    private Vector3 startInput;
    private Vector3 endInPut;
    [SerializeField]
    private bool autoMoveBlock = false;
    [SerializeField]
    private bool triggerAutoMoveBlock = false;
    private bool isWinGame = false;

    private const int MAX_HINT= 30;

  

    private Vector2[,] mGridPos;


    public List<GridContainer> GridInGame
    {
        get
        {
            return gridInGame;
        }
    }


  

    private void Awake()
    {
        instance = this;
        InitGrid();

        nextHintBtn.onClick.AddListener(doTheHint);
        preHintBtn.onClick.AddListener(unDoHint);

         
    }


    private void InitGrid()
    {
        mGridPos = new Vector2[4, 5];
        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 5; y++)
            {
                mGridPos[x, y] = new Vector2(x, y);
            }
        }
    }

    public Vector2[,] GetGridPos()
    {
        return mGridPos;
    }


   

    private bool isCollision = false;
   

    public bool checkIsInHitOrNormalMove(Vector3 lastPosition)
    {
        if (isHint)   //如果是提示状态，则木块到达提示状态才会继续下一步
        {
            int startX = Mathf.RoundToInt(lastPosition.x);
            int startY = Mathf.RoundToInt(lastPosition.y);

            int shadowX = shadowBlock.GetIntPositionX;
            int shadowY = shadowBlock.GetIntPositionY;

            if (startX == shadowX && startY == shadowY)
            {
                Level level = GameManager.instance.CurrentLevel;
                countHint++;
                if (countHint >= level.hintContainer.hints.Count|| countHint >=MAX_HINT)
                {
                    FinishHintGame();
                    hintStepText.text = (countHint).ToString() + "/" + (level.hintContainer.hints.Count).ToString();
                }
                else
                {
                    int position = level.hintContainer.hints[countHint].blockIndex;
                    ShowHintGame(position, countHint);
                }
                SoundManager.instance.BlockPlace(true);   //到达终点
           
                return true;
            }
            else
            {
                SoundManager.instance.BlockPlace(true);    //到达终点
                return false;
            }


        }
        else
        {
            SoundManager.instance.BlockPlace(true);//到达终点
          
            return true;
        }
    }


    public void AddMove(BlockObj currentBlockObj,GameObject block, Vector3 oldPosition, Vector3 newPosition)
    {
        //Undo Action
        UndoAction undoAction = new UndoAction();
        undoAction.block = block;
        undoAction.currentBlockObj = currentBlockObj;
        undoAction.prevPosition = oldPosition;
        undoActions.Add(undoAction);
        if (!isHint)
            buttonsInGame[2].interactable = true;
        GameManager.instance.UpdateMove(false);
    }

    private Dictionary<HEROID, bool> isHeroUsed;
    private BlockObj GetBlockObj(int width,int height, bool blockSpecialType,bool isHitSpecial=false,int verticalIndex=0,int horizontalIndex=0,bool isHintBlock=false)
    {
        int type = GetBlockType(width, height, blockSpecialType);
        int prefabType = type+ verticalIndex;

        if (isHintBlock)
        {
            if (type == 0)
            {
                prefabType = 0;
            }
            else if (type == 1)
            {
                prefabType = 1;
            }
            else if (type == 2)
            {
                prefabType = 6;
            }
            else if (type == 3)
            {
                prefabType = 10;
            }
        }

        BlockObj blockObj = null;

        switch (type)
        {
            case 0:
                prefabType = 0;
                for (int i = 0; i < blockObjsWidth1.Count; i++)
                {
                    if (!blockObjsWidth1[i].gameObject.activeInHierarchy)
                    {
                        blockObj= blockObjsWidth1[i];
                    }
                }
                break;
            case 1: //横2

                if (isHeroUsed.ContainsKey(HEROID.GUANYU) == false)
                {
                    prefabType = 1;
                    isHeroUsed[HEROID.GUANYU] = true;
                    for (int i = 0; i < blockObjsWidth2a.Count; i++)
                    {
                        if (!blockObjsWidth2a[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsWidth2a[i];
                        }
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.ZHANGFEI) == false)
                {
                    prefabType = 2;
                    isHeroUsed[HEROID.ZHANGFEI] = true;
                    for (int i = 0; i < blockObjsWidth2b.Count; i++)
                    {
                        if (!blockObjsWidth2b[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsWidth2b[i];
                        }
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.ZHOYUN) == false)
                {
                    prefabType = 3;
                    isHeroUsed[HEROID.ZHOYUN] = true;
                    for (int i = 0; i < blockObjsWidth2c.Count; i++)
                    {
                        if (!blockObjsWidth2c[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsWidth2c[i];
                        }
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.MACHAO) == false)
                {
                    prefabType = 4;
                    isHeroUsed[HEROID.MACHAO] = true;
                    for (int i = 0; i < blockObjsWidth2d.Count; i++)
                    {
                        if (!blockObjsWidth2d[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsWidth2d[i];
                        }
                    }
                    
                }
                else if (isHeroUsed.ContainsKey(HEROID.HUANGZHONG) == false)
                {
                    prefabType = 5;
                    isHeroUsed[HEROID.HUANGZHONG] = true;
                    for (int i = 0; i < blockObjsWidth2e.Count; i++)
                    {
                        if (!blockObjsWidth2e[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsWidth2e[i];
                        }
                    }
                  
                }
                break;
            case 2:  //竖状，补充关羽
               if (isHeroUsed.ContainsKey(HEROID.ZHANGFEI) == false)
                {
            
                    prefabType = 6;
                    isHeroUsed[HEROID.ZHANGFEI] = true;
                    for (int i = 0; i < blockObjsHeight2a.Count; i++)
                    {
                        if (!blockObjsHeight2a[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsHeight2a[i];
                        }
                    }
                  
                }
                else if (isHeroUsed.ContainsKey(HEROID.ZHOYUN) == false)
                    {
                        prefabType = 7;
                    isHeroUsed[HEROID.ZHOYUN] = true;
                    for (int i = 0; i < blockObjsHeight2b.Count; i++)
                    {
                        if (!blockObjsHeight2b[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsHeight2b[i];
                        }
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.MACHAO) == false)
                {
                    prefabType = 8;
                    isHeroUsed[HEROID.MACHAO] = true;
                    for (int i = 0; i < blockObjsHeight2c.Count; i++)
                    {
                        if (!blockObjsHeight2c[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsHeight2c[i];
                        }
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.HUANGZHONG) == false)
                {
                    prefabType = 9;
                    isHeroUsed[HEROID.HUANGZHONG] = true;
                    for (int i = 0; i < blockObjsHeight2d.Count; i++)
                    {
                        if (!blockObjsHeight2d[i].gameObject.activeInHierarchy)
                        {
                            blockObj= blockObjsHeight2d[i];
                        }
                    }
                   
                }else if (isHeroUsed.ContainsKey(HEROID.GUANYU) == false)
                {

                    prefabType = 11;
                    isHeroUsed[HEROID.GUANYU] = true;
                    for (int i = 0; i < blockObjsHeight2e.Count; i++)
                    {
                        if (!blockObjsHeight2e[i].gameObject.activeInHierarchy)
                        {
                            blockObj = blockObjsHeight2e[i];
                        }
                    }

                }

                        break;
            case 3:
                prefabType = 10;
                if (isHitSpecial==false)
                {
                    for (int i = 0; i < blockObjSpeicals.Count; i++)
                    {
                        if (!blockObjSpeicals[i].gameObject.activeInHierarchy)
                        {
                            blockObj = blockObjSpeicals[i];
                        }
                    }
                }
                break;
        }


        if (blockObj == null)
        {
              GameObject obj = Instantiate(blocksPrefab[prefabType]);
              obj.transform.SetParent(blockContainer, false);

                blockObj = obj.GetComponent<BlockObj>();
                switch (prefabType)
                {
                    case 0:
                        blockObjsWidth1.Add(blockObj);
                        break;
                    case 1:
                         blockObjsWidth2a.Add(blockObj);
                        break;
                    case 2:
                        blockObjsWidth2b.Add(blockObj);
                        break;
                    case 3:
                        blockObjsWidth2c.Add(blockObj);
                        break;
                    case 4:
                        blockObjsWidth2d.Add(blockObj);
                        break;
                    case 5:
                        blockObjsWidth2e.Add(blockObj);
                         break;
                    case 6:
                        blockObjsHeight2a.Add(blockObj);
                        break;
                    case 7:
                        blockObjsHeight2b.Add(blockObj);
                        break;
                    case 8:
                        blockObjsHeight2c.Add(blockObj);
                        break;
                    case 9:
                        blockObjsHeight2d.Add(blockObj);
                        break;
                case 11:
                        blockObjsHeight2e.Add(blockObj);
                        break;
                case 10:
                        if (isHitSpecial == false)
                        {
                             blockObjSpeicals.Add(blockObj);
                        }
                        break;
                }
        }

        blockObj.VisibleImageObj(true);
        blockObj.VisibleHintObj(false);
        blockObj.VisibleFade(false);

        blockObj.GetComponent<BoxCollider2D>().enabled = true;


        return blockObj;
    }

  
    private int GetBlockType(int width,int height, bool blockSpecialType)
    {

        if (blockSpecialType)
        {
            return 3;
        }
        //Get Block Type
        int type = 0; //default block width 2
        if (width == 1 && height == 1)
        {
            type = 0;  //1格
        } else if (width == 2 && height == 1)
        {
            type = 1;  //横两格
        }else if (width == 1 && height == 2)
        {
            type = 2;  //竖两格
        }
        else if (width == 2 && height == 2)
        {
            type = 3;  //竖两格
        }

        return type;
    }

    private void ShowHintGame(int position,int numberHint)
    {
        for (int i = 0; i < blockObjsInGame.Count; i++)
        {
            blockObjsInGame[i].VisibleFade(true);
        }
       
        blockObjsInGame[position].VisibleFade(false);
        Level level = GameManager.instance.CurrentLevel;

        if(shadowBlock)
        {   
            shadowBlock.gameObject.SetActive(false);
            shadowBlock.VisibleImageObj(true);
            shadowBlock.VisibleHintObj(false);
        }
        // shadowBlock = GetBlockObj(blockObjsInGame[position].GetHorizontal, blockObjsInGame[position].GetBlockRange, false);
        shadowBlock = GetBlockObj(blockObjsInGame[position].width, blockObjsInGame[position].height, blockObjsInGame[position].blockMain, blockObjsInGame[position].isHitSpecial, blockObjsInGame[position].verticalIndex, blockObjsInGame[position].horizontalIndex,true);
        shadowBlock.name = "Block_Shadow_"+blockObjsInGame[position].name;
        shadowBlock.VisibleHintObj(true);
        shadowBlock.VisibleFade(false);
        shadowBlock.VisibleImageObj(false);
        shadowBlock.gameObject.SetActive(true);
        shadowBlock.GetComponent<BoxCollider2D>().enabled = false;

        HintInfor hintInfor = level.hintContainer.hints[numberHint];
        shadowBlock.transform.localPosition = blockObjsInGame[position].GetHintPosition(hintInfor.numberMoveRow, hintInfor.numberMoveCol);
        HandMove.instance.DeActiveHandMove();
        HandMove.instance.ActiveHandMove(blockObjsInGame[position].transform.position, shadowBlock.transform.position);

        hintStepText.text = (numberHint).ToString() + "/" + (level.hintContainer.hints.Count).ToString();


        if (numberHint == 0)
        {
            preHintBtn.gameObject.SetActive(false);
        }
        else
        {
            preHintBtn.gameObject.SetActive(true);
        }

        hintContent.gameObject.SetActive(true);


    }

    public void WinGame()
    {
        FinishHintGame();

        for (int i = 0; i <blockObjsInGame.Count ; i++)
        {
            blockObjsInGame[i].gameObject.SetActive(false);
        }

        VisibleButton(false);
        GameManager.instance.WinGame();


#if UNITY_IOS || GOOGLEPLAY
        LeaderboardManager.getInstance().Authenticate(LeaderboarMode.UPLOAD_SCORE);
#else
        if (string.IsNullOrEmpty(PlayFabAuthServiceForManager.Instance.RememberMeDisplayNameId) == false)
        {   //已经记录过昵称
            //胜利后上传排行榜
            LeaderboardManager.getInstance().Authenticate(LeaderboarMode.UPLOAD_SCORE);
        }
#endif


        isWinGame = true;
        GameProgressTracker.DeleteGameProgress();
        BoosterManager.instance.isCanSave = false;
    }

    private void VisibleButton(bool visible)
    {
        for (int i = 0; i < buttonsInGame.Count; i++)
        {
            buttonsInGame[i].interactable = visible;
        }
    }

    public void FinishHintGame()
    {
        if (isHint)
        {
            for (int i = 0; i < blockObjsInGame.Count; i++)
            {
                blockObjsInGame[i].VisibleFade(false);
            }
            isHint = false;
            countHint = 0;
            VisibleButton(true);
            HandMove.instance.DeActiveHandMove();
            if (shadowBlock != null)
            {
                shadowBlock.gameObject.SetActive(false);
            }

            hintContent.gameObject.SetActive(false);
        }
       
    }

    public void doTheHint()
    {
        //执行步骤
        int shadowX = shadowBlock.GetIntPositionX;
        int shadowY = shadowBlock.GetIntPositionY;

        Level level = GameManager.instance.CurrentLevel;
        int position = level.hintContainer.hints[countHint].blockIndex;

        BlockObj currentBlockObj = blockObjsInGame[position];
        Vector2 newPosition = new Vector2(shadowX, shadowY);

        PlayingManager.instance.AddMove(currentBlockObj,currentBlockObj.gameObject, currentBlockObj.transform.localPosition, newPosition);

        blockObjsInGame[position].transform.localPosition = newPosition;
        currentBlockObj.savePosition = newPosition;

        SoundManager.instance.BlockPlace(true);   //到达终点

        //执行步骤后
        countHint++;
        if (countHint >= level.hintContainer.hints.Count|| countHint >= MAX_HINT)
        {
            FinishHintGame();
            hintStepText.text = (countHint).ToString() + "/" + (level.hintContainer.hints.Count).ToString();
        }
        else
        {
            position = level.hintContainer.hints[countHint].blockIndex;
            ShowHintGame(position, countHint);
        }
       

    }

    //回退提示
    public void unDoHint()
    {
        UndoGame();
        SoundManager.instance.BlockPlace(isCollision);   //到达终点

        countHint--;

        Level level = GameManager.instance.CurrentLevel;
        int position = level.hintContainer.hints[countHint].blockIndex;
        ShowHintGame(position, countHint);

    }

    Level currentLevel;
    public void PlayGame(float waitTime  =0.06f)
    {
       

        AdManager.Instance.IsInterstitialAvailable();
        isPause = false;
       
         blockObjsInGame.Clear();
         listStartPosition.Clear();

        isHeroUsed=new Dictionary<HEROID, bool>();
        InputManager.Instance.DisableTouch();
        BoosterManager.instance.IsMoveBeginGame = false;
        Timer.Schedule(this, waitTime, () =>
        {
            Reset();
            currentLevel = GameManager.instance.CurrentLevel;
            for (int i = 0; i < currentLevel.levelData.blockInfors.Count; i++)
            {
                BlockInfor infor = currentLevel.levelData.blockInfors[i];
                if (i == 0)
                {
                    infor.blockMain = true;
                }

                int veritcalIndex = 0;
                if (infor.blockStyle == 3)
                {
                    veritcalIndex = infor.charName - 'H';
                }

                int horizontalIndex = 0;
                if (infor.blockStyle == 2)
                {
                    horizontalIndex = infor.charName - 'B';
                }


                BlockObj blockObj = GetBlockObj(infor.width, infor.height, infor.blockMain,false, veritcalIndex, horizontalIndex);

               
                blockObj.width = infor.width;
                blockObj.height = infor.height;
                blockObj.blockMain = infor.blockMain;
                blockObj.isHitSpecial = false;
                blockObj.verticalIndex = veritcalIndex;
                blockObj.horizontalIndex = horizontalIndex;
                blockObj.charName = infor.charName;
                blockObj.blockStyle = infor.blockStyle;

                blockObj.gameObject.SetActive(true);
              
                if (infor.y >= GameManager.gridHeight) infor.y = GameManager.gridHeight - 1;
                if (infor.x >= GameManager.gridWidth) infor.x = GameManager.gridWidth - 1;

                int x = infor.x;
               
                blockObj.name = string.Format("Block_{0}", (i));
                Vector3 position = new Vector3(x, infor.y, 0);

                blockObj.savePosition = position;

                blockObj.SetUp(position,i+1);
              
                blockObjsInGame.Add(blockObj);
                listStartPosition.Add(position);
            }
            GameProgressTracker.DeleteGameProgress();
            BoosterManager.instance.isCanSave = true;
        });
 
    }

    //保存进度
    public ProgressData progressDataToSave()
    {
        ProgressData progressData = new ProgressData();

        progressData.blocksInGameToSave = new BlockContainerToSave();
        BlockObjToSave blockObjToSave = null;
      
        if (blockObjsInGame.Count > 0)
        {
            foreach (BlockObj blockObj in blockObjsInGame)
            {
                blockObjToSave = new BlockObjToSave();

                blockObjToSave.width = blockObj.width;
                blockObjToSave.height = blockObj.height;
                blockObjToSave.blockMain = blockObj.blockMain;
                blockObjToSave.isHitSpecial = blockObj.isHitSpecial;
                blockObjToSave.verticalIndex = blockObj.verticalIndex;
                blockObjToSave.horizontalIndex = blockObj.horizontalIndex;
                blockObjToSave.charName = blockObj.charName;
                blockObjToSave.blockStyle = blockObj.blockStyle;
                blockObjToSave.localPosition = blockObj.savePosition;


                progressData.blocksInGameToSave.blocks.Add(blockObjToSave);
            }
        }

        List<Vector3> savePosition = new List<Vector3>();
        foreach (Vector3 position in listStartPosition)
        {
            savePosition.Add(position);
        }


        progressData.listStartPosition = savePosition;
        progressData.GameMode=GameManager.GAME_MODE;
        progressData.movestep = GameManager.instance.move;
        progressData.GameId=(int)GameManager.instance.levelGameMode;
        progressData.levelState = currentLevel.levelStage-1;

        return progressData;
    }


    public void ResumeGame(float waitTime = 0.06f)
    {

        ProgressData gameProgressData = GameProgressTracker.GetGameProgress();

        BlockContainerToSave blockContainerToSave = gameProgressData.blocksInGameToSave;
        List<BlockObjToSave> blocks = blockContainerToSave.blocks;

        isPause = false;
        listStartPosition.Clear();
        blockObjsInGame.Clear();
        isHeroUsed = new Dictionary<HEROID, bool>();

        GameManager.GAME_MODE = gameProgressData.GameMode;
        GameManager.instance.SelectedGameMode((GAME_MODE_ID)gameProgressData.GameId);

        currentLevel = GameManager.instance.AllLevels[(GAME_MODE_ID)gameProgressData.GameId][gameProgressData.levelState];
        currentLevel.Load();
        GameManager.instance.CurrentLevel = currentLevel;

        InputManager.Instance.DisableTouch();
        BoosterManager.instance.IsMoveBeginGame = false;
        Timer.Schedule(this, waitTime, () =>
        {
            Reset();
            for (int i = 0; i < blocks.Count; i++)
            {

                BlockObjToSave blockObjFromSave = blocks[i];

                BlockObj blockObj = GetBlockObj(blockObjFromSave.width, blockObjFromSave.height, blockObjFromSave.blockMain, false, blockObjFromSave.verticalIndex, blockObjFromSave.horizontalIndex);

           
                blockObj.width = blockObjFromSave.width;
                blockObj.height = blockObjFromSave.height;
                blockObj.blockMain = blockObjFromSave.blockMain;
                blockObj.isHitSpecial = false;
                blockObj.verticalIndex = blockObjFromSave.verticalIndex;
                blockObj.horizontalIndex = blockObjFromSave.horizontalIndex;
                blockObj.charName = blockObjFromSave.charName;
                blockObj.blockStyle = blockObjFromSave.blockStyle;
                blockObj.transform.localPosition = blockObjFromSave.localPosition;
                blockObj.savePosition= blockObjFromSave.localPosition;

                blockObj.gameObject.SetActive(true);

                blockObj.name = string.Format("Block_{0}", (i));
                blockObj.SetUp(blockObjFromSave.localPosition, i + 1);
                blockObjsInGame.Add(blockObj);

            }

            List<Vector3> savePosition = gameProgressData.listStartPosition;
            foreach (Vector3 position in savePosition)
            {
                listStartPosition.Add(position);
            }

            GameManager.instance.PlayGame(gameProgressData.movestep);

            BoosterManager.instance.isCanSave = true;
        });


    }

    public void Reset(bool move = false)
    {
       
        isWinGame = false;
        arrowImage.SetActive(true);
        BoosterManager.instance.IsMoveBeginGame = move;
        if (!move)
        {
            VisibleButton(false);
        }
        undoActions.Clear();
        buttonsInGame[2].interactable = false;
        isHint = false;
        countHint = 0;
        //CLear All Obj In Grid
        selectedBlock = null;
        if (shadowBlock != null)
        {
            shadowBlock.gameObject.SetActive(false);
            shadowBlock = null;
        }

        // Debug.Log("BlockObj-- >speedMoveBlock " + GameManager.speedMoveBlock);
        // Debug.Log("BlockObj-- >blockInfors " + GameManager.instance.CurrentLevel.levelData.blockInfors.Count);
        // Debug.Log("BlockObj-- >time delay " + (GameManager.speedMoveBlock - 0.05f) * ((float)GameManager.instance.CurrentLevel.levelData.blockInfors.Count / 1.7f));


        // Timer.Schedule(this, (GameManager.speedMoveBlock-0.05f) * ((float)GameManager.instance.CurrentLevel.levelData.blockInfors.Count/1.7f )+0.5f, () =>
        Timer.Schedule(this, GameManager.speedMoveBlock+(float)GameManager.instance.CurrentLevel.levelData.blockInfors.Count*0.2f, () =>
        {

            BoosterManager.instance.IsMoveBeginGame = true;
            VisibleButton(BoosterManager.instance.IsMoveBeginGame);
            InputManager.Instance.EnableTouch();
        });

    }
    public void HintGame()
    {
        //动态生成提示步骤：
        char[] stateBoard = new char[20];
        //初始化，从上到下从左到右边
        for (int i = 0; i < 20; i++)
        {
            stateBoard[i] = '@';
        }

        foreach (BlockObj blockObj in blockObjsInGame)
        {
            int posX = blockObj.GetIntPositionX;
            int Y = blockObj.GetIntPositionY;
            int posY = 4 - Y;
            int blockStyle = blockObj.blockStyle;

            int pos = posX + posY * klotskiShare.G_BOARD_X;

            switch (blockStyle)
            {
                case 0: //empty block
                    break;
                case 1: // 1X1 block
                    stateBoard[pos] = blockObj.charName;
                    break;
                case 2: // 2X1 block
                    stateBoard[pos] = blockObj.charName;
                    stateBoard[pos+1] = blockObj.charName;
                    break;
                case 3: // 1X2 block
                    stateBoard[pos] = blockObj.charName;
                    stateBoard[pos- klotskiShare.G_BOARD_X] = blockObj.charName;
                    break;
                case 4: // 2X2 block
                    stateBoard[pos] = blockObj.charName;
                    stateBoard[pos+1] = blockObj.charName;
                    stateBoard[pos - klotskiShare.G_BOARD_X] = blockObj.charName;
                    stateBoard[pos - klotskiShare.G_BOARD_X+1] = blockObj.charName;
                    break;
                default:
                    Debug.LogError("key2Board(): design error !");
                    break;
            }
        }

        Debug.Log("HintGame-->startBoard:\n" + showBoard(stateBoard));

        Dictionary<int, int> blockValueBlockIndexDic=new Dictionary<int, int>();

        Level currentLevel = GameManager.instance.CurrentLevel;
        List<BlockInfor> blockInfors = currentLevel.levelData.blockInfors;
        for (int i = 0; i < blockInfors.Count; i++)
        {
            int blockvalue = (int)(blockInfors[i].charName - klotskiShare.G_VOID_CHAR);
            blockValueBlockIndexDic[blockvalue] =i ;
        }
       

        List<HintInfor> runtimeHints = klotskiHints.getHintList(stateBoard, blockValueBlockIndexDic);
        if (runtimeHints.Count == 0)
        {
            runtimeHints = new List<HintInfor>();
            HintInfor hintInfor=new HintInfor();
            hintInfor.blockIndex = 0;
            hintInfor.numberMoveCol = 0;
            hintInfor.numberMoveRow = -1;
            runtimeHints.Add(hintInfor);
       }

        if (runtimeHints.Count > 0)
        {
            ///    ReplayGame(false);
            ///    UpdateAllGrid();
            Level level = GameManager.instance.CurrentLevel;
            level.hintContainer.hints = runtimeHints;
            int position = level.hintContainer.hints[0].blockIndex;
            buttonsInGame[1].interactable = false;
            buttonsInGame[2].interactable = false;
            buttonsInGame[3].interactable = false;
            isHint = true;
            countHint = 0;
            ShowHintGame(position, 0);
        }
        else
        {
            Toast.instance.ShowMessage("曹操已经在出口位置");
        }
   
    }

    private static string showBoard(char[] charBoard)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < charBoard.Length; i++)
        {
            if (i != 0 && i % 4 == 0)
            {
                sb.Append("\n");
            }

            sb.Append(charBoard[i]);

        }
        return sb.ToString();
    }

   
   
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            GameProgressTracker.SaveProgress();
        }
        
    }

    public void HideAllBlock()
    {
        if (isWinGame == false)
        {
            GameProgressTracker.SaveProgress();
        }

        for (int i = 0; i < blockObjsInGame.Count; i++)
        {
            blockObjsInGame[i].DeActiveMoveEffect();
            blockObjsInGame[i].gameObject.SetActive(false);
        }

        FinishHintGame();

        if (shadowBlock != null)
        {
            shadowBlock.gameObject.SetActive(false);
            shadowBlock = null;
        }
        HandMove.instance.DeActiveHandMove();

       
    }

    public void ReplayGame(bool move =true)
    {
        Reset(!move);
        ClearAllGrid();
        for (int i = 0; i < blockObjsInGame.Count; i++)
        {
            blockObjsInGame[i].SetUp(listStartPosition[i],i+1, move);
            blockObjsInGame[i].gameObject.SetActive(true);
          
        }

        HandMove.instance.DeActiveHandMove();
        if (shadowBlock != null)
        {
            shadowBlock.gameObject.SetActive(false);
        }
       
      
        GameManager.instance.RePlayGame();
        BoosterManager.instance.isCanSave = true;
    }

    public void OnEventPause(bool pause)
    {
        isPause = pause;
    }

    
    public void UndoGame()
    {
        if(BlockObj.islocking == false) 
        {
            if (undoActions.Count > 0)
            {  
                undoActions[undoActions.Count - 1].Action();
                undoActions.RemoveAt(undoActions.Count - 1);
            
                UpdateAllGrid();
                GameManager.instance.UpdateMove(true);
            }
            if (undoActions.Count <= 0)
            {
                buttonsInGame[2].interactable = false;
            }
        }
    }

    private void UpdateAllGrid()
    {
        ClearAllGrid();
        for (int i = 0; i < blockObjsInGame.Count; i++)
        {
            blockObjsInGame[i].UpdateBoxGrid();
        }
    }

    private void ClearAllGrid()
    {
        for (int i = 0; i < GameManager.gridHeight; i++)
        {

            for (int j = 0; j < GameManager.gridWidth; j++)
            {
                gridInGame[i].grids[j] = null;
            }

        }
    }
 
}
