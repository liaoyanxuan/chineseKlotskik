using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScrollRectFrameWork;
using UnityEngine.UI;
using ScreenFrameWork;
using TMPro;
using Hyperbyte;

public class LevelListItem : MonoBehaviour
{
 
    [SerializeField]
    private TextMeshProUGUI levelText ;
    [SerializeField]
    private GameObject starObj;
    [SerializeField]
    private GameObject completeObj;
    [SerializeField]
    private GameObject lockedObj;
    [SerializeField]
    private GameObject fadeObj;
    [SerializeField]
    private GameObject highLightObj;

    [SerializeField]
    private Transform blockContainer;
    [SerializeField]
    private GameObject[] starsLight;
 

    [Space()]
 
 
    [SerializeField]
    private List<GridContainer> gridContainers = new List<GridContainer>();
 
    [SerializeField]
    private bool locked = false;
    [SerializeField]
    private bool current = false;
    [SerializeField]
    private bool completed = false;

    private Level dataObject;
    [SerializeField]

    private List<GameObject> blocksPrefab = new List<GameObject>();
    [SerializeField]
    private List<BlockLevelShow> blockObjsInLevel = new List<BlockLevelShow>();
    private List<BlockLevelShow> blockObjsWidth1 = new List<BlockLevelShow>();
    private List<BlockLevelShow> blockObjsWidth2a = new List<BlockLevelShow>();//关羽-横
    private List<BlockLevelShow> blockObjsWidth2b = new List<BlockLevelShow>(); //张飞-横
    private List<BlockLevelShow> blockObjsWidth2c = new List<BlockLevelShow>();//赵云-横
    private List<BlockLevelShow> blockObjsWidth2d = new List<BlockLevelShow>();//马超-横
    private List<BlockLevelShow> blockObjsWidth2e = new List<BlockLevelShow>();//黄忠-横
    private List<BlockLevelShow> blockObjsHeight2a = new List<BlockLevelShow>();//张飞-竖
    private List<BlockLevelShow> blockObjsHeight2b = new List<BlockLevelShow>();//赵云-竖
    private List<BlockLevelShow> blockObjsHeight2c = new List<BlockLevelShow>();//马超-竖
    private List<BlockLevelShow> blockObjsHeight2d = new List<BlockLevelShow>();//黄忠-竖
    private List<BlockLevelShow> blockObjsHeight2e = new List<BlockLevelShow>();//关羽-竖

    private BlockLevelShow blockSpecial;
 

    public  void Setup(Level dataObject)
    {


        VisibleBlocks(false);
        dataObject.Load();
        levelText.text = dataObject.levelShowStage.ToString()+"."+ dataObject.levelData.blockInfors[0].blockname;

        dataObject.Load();
        this.dataObject = dataObject;
        SetUpLevel(dataObject);
        SetUpBoard(dataObject.levelData);

    }

    private void SetUpLevel(Level dataObject)
    {
        int levelCurrent = GameManager.instance.GetLevelCompleteMode(dataObject.gameMode) ;
        
        int starEarn = dataObject.GetStar();

        locked = dataObject.levelStage > (levelCurrent);

        //提前解锁
        if (dataObject.GetUnlock() == 1)
        {
            locked = false;
        }

        completed = levelCurrent > dataObject.levelStage;

        if (dataObject.GetStar() > 0)
        {
            completed = true;
        }


        current = levelCurrent == dataObject.levelStage;

        lockedObj.SetActive(locked);
        fadeObj.SetActive(locked);
        starObj.SetActive(completed); //是否已经完成，未完成不显示星星数
        completeObj.SetActive(completed);
        highLightObj.SetActive(current);
        if (completed)
        {

            for (int i = 0; i < starsLight.Length; i++)
            {
                starsLight[i].transform.parent.gameObject.SetActive(false);

                starsLight[i].SetActive(true);
            }

            int mini = Mathf.CeilToInt(dataObject.levelData.blockInfors[0].mini/10f);
            for (int i = 0; i < mini; i++)
            {
                starsLight[i].transform.parent.gameObject.SetActive(true);
            }

            for (int i = starEarn; i < starsLight.Length; i++)
            {
                starsLight[i].SetActive(false);
            }
        }
    }

    private void SetUpBoard(LevelData levelData)
    {
        isHeroUsed = new Dictionary<HEROID, bool>();
        Timer.Schedule(this, .02f, () =>
        {
            for (int i = 0; i < levelData.blockInfors.Count; i++)  //最后一个没用
            {
                BlockInfor infor = levelData.blockInfors[i];
                if(i==0)
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

                BlockLevelShow blockObj = GetBlockObj(infor.GetWidth(), infor.GetHeight(), infor.blockMain, veritcalIndex, horizontalIndex);
                blockObjsInLevel.Add(blockObj);
              
                if (infor.y >= GameManager.gridHeight) infor.y = GameManager.gridHeight - 1;
                if (infor.x >= GameManager.gridWidth) infor.x = GameManager.gridWidth - 1;

                blockObj.gameObject.SetActive(true);
                blockObj.transform.SetParent(blockContainer, false);
                int x = infor.x;
                blockObj.transform.localPosition = gridContainers[infor.y].grids[x].transform.localPosition;

            }
        });
    }





    private Dictionary<HEROID, bool> isHeroUsed;
    private BlockLevelShow GetBlockObj(int width, int height, bool blockSpecialType, int verticalIndex = 0, int horizontalIndex = 0)
    {
        int type = GetBlockType(width, height, blockSpecialType);
        int prefabType = type + verticalIndex;

        switch (type)
        {
            case 0:
                prefabType = 0;
                for (int i = 0; i < blockObjsWidth1.Count; i++)
                {
                    if (!blockObjsWidth1[i].gameObject.activeInHierarchy) return blockObjsWidth1[i];
                }
                break;
            case 1: //横2

                if (isHeroUsed.ContainsKey(HEROID.GUANYU) == false)
                {
                    prefabType = 1;
                    isHeroUsed[HEROID.GUANYU] = true;
                    for (int i = 0; i < blockObjsWidth2a.Count; i++)
                    {
                        if (!blockObjsWidth2a[i].gameObject.activeInHierarchy) return blockObjsWidth2a[i];
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.ZHANGFEI) == false)
                {
                    prefabType = 2;
                    isHeroUsed[HEROID.ZHANGFEI] = true;
                    for (int i = 0; i < blockObjsWidth2b.Count; i++)
                    {
                        if (!blockObjsWidth2b[i].gameObject.activeInHierarchy) return blockObjsWidth2b[i];
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.ZHOYUN) == false)
                {
                    prefabType = 3;
                    isHeroUsed[HEROID.ZHOYUN] = true;
                    for (int i = 0; i < blockObjsWidth2c.Count; i++)
                    {
                        if (!blockObjsWidth2c[i].gameObject.activeInHierarchy) return blockObjsWidth2c[i];
                    }
                  
                }
                else if (isHeroUsed.ContainsKey(HEROID.MACHAO) == false)
                {
                    prefabType = 4;
                    isHeroUsed[HEROID.MACHAO] = true;
                    for (int i = 0; i < blockObjsWidth2d.Count; i++)
                    {
                        if (!blockObjsWidth2d[i].gameObject.activeInHierarchy) return blockObjsWidth2d[i];
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.HUANGZHONG) == false)
                {
                    prefabType = 5;
                    isHeroUsed[HEROID.HUANGZHONG] = true;
                    for (int i = 0; i < blockObjsWidth2e.Count; i++)
                    {
                        if (!blockObjsWidth2e[i].gameObject.activeInHierarchy) return blockObjsWidth2e[i];
                    }
                  
                }
                break;
            case 2:
                if (isHeroUsed.ContainsKey(HEROID.ZHANGFEI) == false)
                {

                    prefabType = 6;
                    isHeroUsed[HEROID.ZHANGFEI] = true;
                    for (int i = 0; i < blockObjsHeight2a.Count; i++)
                    {
                        if (!blockObjsHeight2a[i].gameObject.activeInHierarchy) return blockObjsHeight2a[i];
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.ZHOYUN) == false)
                {
                    prefabType = 7;
                    isHeroUsed[HEROID.ZHOYUN] = true;
                    for (int i = 0; i < blockObjsHeight2b.Count; i++)
                    {
                        if (!blockObjsHeight2b[i].gameObject.activeInHierarchy) return blockObjsHeight2b[i];
                    }
                  
                }
                else if (isHeroUsed.ContainsKey(HEROID.MACHAO) == false)
                {
                    prefabType = 8;
                    isHeroUsed[HEROID.MACHAO] = true;
                    for (int i = 0; i < blockObjsHeight2c.Count; i++)
                    {
                        if (!blockObjsHeight2c[i].gameObject.activeInHierarchy) return blockObjsHeight2c[i];
                    }
                   
                }
                else if (isHeroUsed.ContainsKey(HEROID.HUANGZHONG) == false)
                {
                    prefabType = 9;
                    isHeroUsed[HEROID.HUANGZHONG] = true;
                    for (int i = 0; i < blockObjsHeight2d.Count; i++)
                    {
                        if (!blockObjsHeight2d[i].gameObject.activeInHierarchy) return blockObjsHeight2d[i];
                    }

                }
                else if(isHeroUsed.ContainsKey(HEROID.GUANYU) == false)
                {

                    prefabType = 11;
                    isHeroUsed[HEROID.GUANYU] = true;
                    for (int i = 0; i < blockObjsHeight2e.Count; i++)
                    {
                        if (!blockObjsHeight2e[i].gameObject.activeInHierarchy) return blockObjsHeight2e[i];
                    }

                }
                    break;
            case 3:
                prefabType = 10;
                if (this.blockSpecial != null)
                {
                    return this.blockSpecial;
                }
                break;
        }



        GameObject obj = Instantiate(blocksPrefab[prefabType]);

        obj.transform.SetParent(blockContainer, false);
        BlockLevelShow blockObj = obj.GetComponent<BlockLevelShow>();
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
                this.blockSpecial = blockObj;
                break;
        }


        return blockObj;
    }

    private int GetBlockType(int width, int height, bool blockSpecialType)
    {

        if (blockSpecialType)
        {
            return 3;
        }
        //Get Block Type
        int type = 3; //default block width 2
        if (width == 1 && height == 1)
        {
            type = 0;  //1格
        }
        else if (width == 2 && height == 1)
        {
            type = 1;  //横两格
        }
        else if (width == 1 && height == 2)
        {
            type = 2;  //竖两格
        }
        

        return type;
    }

  

    private void VisibleBlocks(bool visible)
    {
        for (int i = 0; i < blockObjsInLevel.Count; i++)
        {
            blockObjsInLevel[i].gameObject.SetActive(visible);
        }

        blockObjsInLevel.Clear();
    }


  
    public void OnEventClicked()
    {
        if (locked)
        {
            UIController.Instance.toolUseQuery.Activate();
            UIController.Instance.toolUseQuery.GetComponent<ToolUseQueryScript>().SetToolReason(ToolUseQueryScript.USE_UNLOCK,()=>
            {
                dataObject.SetUnlock();
                ScreenManager.Instance.Show("game");
                GameManager.instance.CurrentLevel = dataObject;
                GameManager.instance.PlayGame();
            }, dataObject.levelData.blockInfors[0].blockname);
        }
        else
        {
            if (GameProgressTracker.HasGameProgress())
            {
                ProgressData progressData= GameProgressTracker.GetGameProgress();
                if ((GAME_MODE_ID)progressData.GameId == dataObject.gameMode && progressData.levelState == dataObject.levelStage - 1)
                {
                    GameManager.instance.isResumeGame = true;
                    ScreenManager.Instance.Show("game");
                    return;
                }
            }

            ScreenManager.Instance.Show("game");
            GameManager.instance.CurrentLevel = dataObject;
            GameManager.instance.PlayGame();
        }
    }


}
