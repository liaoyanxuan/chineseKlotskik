using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAndLoopHelper : MonoBehaviour
{
    /*
    #region Inspector Variables
    [Space]
    [SerializeField] private GameObject achieveItemPrefab = null;
    private GridAndLoop gridAndLoop;
    #endregion

    protected AchieveItemScript[] _itemScripts;

    [SerializeField]
    private int reuseItemNum = 8;

    private List<LevelData> itemData;

    void Start()
    {
        gridAndLoop = this.GetComponent<GridAndLoop>();

        itemData = AchieveManager.Instance.AchieveLevelData;

        if (itemData != null)
        {
            gridAndLoop.dataNum = itemData.Count;
        }
        else
        {
            gridAndLoop.dataNum = 10;
        }

        _itemScripts = new AchieveItemScript[reuseItemNum];

        gridAndLoop.onInitializeItem = updateBankItem;  //item更新回调

        GameObject itemObj = null;
        for (int num = 0; num < reuseItemNum; num++)
        {
            itemObj = Instantiate(achieveItemPrefab);  //item预设
          
            itemObj.name = "achieveItem_" + num;
            itemObj.transform.SetParent(this.transform);
            _itemScripts[num]= itemObj.GetComponent<AchieveItemScript>();
        }

        // gridAndLoop.RePosition();
        gridAndLoop.SetScrollRectPos(0);
    }

    public void updateItems()
    {
        if (gridAndLoop!=null)
        {
            gridAndLoop.RePosition();
           
        }
       
    }

    // Update is called once per frame

    protected void updateBankItem(GameObject item, int wrapIndex, int realIndex)
    {
        if (itemData!=null)
        {
            LevelData levelData  = itemData[realIndex];
            _itemScripts[wrapIndex].setLevelData(levelData);
        }
    }
    */

}
