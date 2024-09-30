using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BlockContainerToSave
{
    public List<BlockObjToSave> blocks = new List<BlockObjToSave>();
}

[System.Serializable]
public class BottomObjToSave
{
    public List<GameObject> objs = new List<GameObject>();
    public int positionY = 0;


}

[System.Serializable]
public class BlockObjToSave
{
    public char charName;
    public int blockStyle;
    public int width;
    public int height;
    public bool blockMain;
    public bool isHitSpecial = false;
    public int verticalIndex = 0;
    public int horizontalIndex = 0;
    public Vector3 localPosition;

}

