/* Creator:
 * Usage:
 * Designing:
 * Remaining problems:
 * Caution:
 * Version:
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

//[ExecuteInEditMode]
public class GridAndLoop : MonoBehaviour
{

    /// <summary>
    /// 是否在脚本Start回调中执行初始化
    /// </summary>
    public bool isInitOnStart = true;

    /// <summary>
    /// 设置Item内容的委托
    /// </summary>
    /// <param name="item">Item对象</param>
    /// <param name="wrapIndex">Item在Grid中的序号</param>
    /// <param name="realIndex">当前Item在List中的序号</param>
    public delegate void OnInitializeItem(GameObject item, int wrapIndex, int realIndex);

    /// <summary>
    /// 排列方式枚举
    /// </summary>
    public enum ArrangeType
    {
        Horizontal=0,//水平排列
        Vertical=1,//垂直排列
    }

    public bool ScrollToCenterOnSelected = false;

    public bool EasyAnimation = false;
    /// <summary>
    /// 记录上一次的点位
    /// </summary>
    bool canEase = false;

    public float EasySpeed = 0.05f;
   
    // animate the game object from -1 to +1 and back
    public float minimum = -1.0F;
    public float maximum = 1.0F;

    // starting value for the Lerp
    static float tempTime = 0.0f;

    /// <summary>
    /// Item的尺寸
    /// </summary>
    public int cell_x = 100, cell_y = 100;

    /// <summary>
    /// Item最小序号
    /// </summary>
    private int minIndex = 0;


    /// <summary>
    /// 数据长度
    /// </summary>
    [SerializeField]
    int _dataNum;
    public int dataNum
    {
        get
        {
            return _dataNum;
        }
        set
        {
            _dataNum = value;
            hasReposition = false;
        }
    }
    
    /// <summary>
    /// 排列方式
    /// </summary>
    public ArrangeType arrangeType=ArrangeType.Horizontal;

    /// <summary>
    /// 行列个数 0表示1列
    /// </summary>
    public int ConstraintCount = 0;



    /// <summary>
    ///当前对象
    /// </summary>
    Transform mTrans;

    /// <summary>
    /// 当前RectTransform对象
    /// </summary>
    RectTransform mRTrans;

    public RectTransform CurrentRTrans
    {
        get
        {
            return mRTrans;
        }
    }

    /// <summary>
    /// ScrollRect
    /// </summary>
    [SerializeField]
    ScrollRectFaster mScroll;

    RectTransform scrollRt;

    /// <summary>
    /// 设置Item的委托
    /// </summary>
    public OnInitializeItem onInitializeItem;
    public ScrollRectFaster CurrentScrollRect
    {
        get
        {
            return mScroll;
        }
    }

    /// <summary>
    /// 滚动方向
    /// </summary>
    bool mHorizontal;
    /// <summary>
    /// 元素链表
    /// </summary>
    List<Transform> mChild=new List<Transform>();
 
    /// <summary>
    /// 显示区域长度或高度的一半
    /// </summary>
    float extents=0;
   
    Vector2 SR_size = Vector2.zero;//SrollRect的尺寸

    Vector2 startAxis;
    [NonSerialized]
    bool hasAwaked = false;
    [NonSerialized]
    bool hasReposition = false;
    [NonSerialized]
    bool hasInitList = false;

    [NonSerialized]
    int _selectIndex =-1;
    [NonSerialized]
    float easeValue_ = 0;
    public float easeValue{
        get
        {
            return easeValue_;
        }
        set
        {
            easeValue_ = value;
        }
    }


    int datarows, datacols = 0;
    int reuse_item_count;

    void Awake()
    {
        if (!hasAwaked)
        {
            mTrans = transform;
            mRTrans = transform.GetComponent<RectTransform>();

            if (mScroll == null)
            {
                mScroll = transform.GetComponentInParent<ScrollRectFaster>();
            }
            if (mScroll != null)
            {
                scrollRt = mScroll.GetComponent<RectTransform>();
                scrollRt.pivot = new Vector2(0.5f, 0.5f);
                mScroll.onValueChanged.AddListener(WrapContent);//添加滚动事件回调
            }

            hasAwaked = true;
        }
    }

    
    int sortByName(Transform a, Transform b) { return string.Compare(a.name, b.name); }

    
    /// <summary>
    /// 初始化mChild链表 在Start方法之前调用一次，以后不再执行
    /// </summary>
    void InitList()
    {
        if (hasInitList == false)
        {
            int i,ChildCount;
            if (ConstraintCount <= 0)
                ConstraintCount = 1;
      
        
            mHorizontal = arrangeType == ArrangeType.Vertical;

            mScroll.horizontal = mHorizontal;
            mScroll.vertical = !mHorizontal;

            mRTrans.pivot = new Vector2(0, 1);//设置panel的中心在左上角
           //RTrans.anchorMax = new Vector2(0, 1);
           //RTrans.anchorMin = new Vector2(0, 1);
     
            mChild.Clear();

            for (i = 0, ChildCount = mTrans.childCount; i < ChildCount; i++)
                mChild.Add(mTrans.GetChild(i));

            InitRowAndCols();

        }
        hasInitList = true;

    }
    void InitChildPosition(int cols, int rows)
    {

        for (int i = 0; i < mChild.Count; i++)
        {
            Transform temp = mChild[i];

            int x = 0, y = 0;//行列号
            if (arrangeType == ArrangeType.Horizontal) { x = i / cols; y = i % cols; }
            else if (arrangeType == ArrangeType.Vertical) { x = i % rows; y = i / rows; }


            temp.localPosition = new Vector2(startAxis.x + y * cell_x, startAxis.y - x * cell_y);
        }

    }

    void InitValue()
    {
        HideUselessChild(dataNum);//隐藏多余复用对象

        if (minIndex > dataNum) minIndex = dataNum;
        reuse_item_count = Mathf.Min(mChild.Count, dataNum);
        if (arrangeType == ArrangeType.Vertical) //垂直排列 则适应行数
        {
            datarows = ConstraintCount;
            datacols = (int)Mathf.Ceil((float)dataNum / (float)datarows);

        }
        else if (arrangeType == ArrangeType.Horizontal) //水平排列则适应列数
        {
            datacols = ConstraintCount;
            datarows = (int)Mathf.Ceil((float)dataNum / (float)datacols);
        }
        RefreshScrollViewSize(_dataNum);


    }





    void InitRowAndCols()
    {
        int imax = mChild.Count;//Item元素数量
        int i;
        int rows = 1, cols = 1;

        startAxis = new Vector2(cell_x/2f,-cell_y/2f);//起始位置
        
        //初始化行列数
        if (arrangeType == ArrangeType.Vertical) //垂直排列 则适应行数
        { 
            rows = ConstraintCount;
            cols = (int)Mathf.Ceil((float)imax / (float)rows);
            
            extents = (float)(cols  * cell_x)* 0.5f;

        }
        else if (arrangeType == ArrangeType.Horizontal) //水平排列则适应列数
        { 
            cols = ConstraintCount; 
            rows = (int)Mathf.Ceil((float)imax / (float)cols); 
            extents = (float)(rows *  cell_y)* 0.5f;

        }
        InitChildPosition(cols,rows);
        InitScrollValue(cols, rows);


    }
    //隐藏多余复用对象
    void HideUselessChild(int dataNum)
    {
        for (int i = mChild.Count; i >0; i--)
        {
            if (i > dataNum)
            {
                Transform temp = mChild[i-1];
                setMyActive(temp.gameObject,false);
            }

        }
    }

    void InitScrollValue(int cols,int rows)
    {
        if (mChild.Count < rows * cols)
        {
            
        }

        if (arrangeType == ArrangeType.Vertical)
        {
            cols = cols - 1;
        }
        else
        {
            rows = rows - 1;
        }
        Vector2 size = new Vector2(cols * cell_x,rows * cell_y);

        //scrollRt.sizeDelta = size;

        SR_size = scrollRt.rect.size;

     

    }


    public Transform getItemByPosY(float posY)
    {
        Transform resultTf = null;
        float topY = 0;
        float bottomY = 0;
        bool targetHit = false;
        if (null != mChild && mChild.Count > 0)
        {
            foreach (Transform tf in mChild)
            {
                topY = tf.localPosition.y + cell_y / 2f;
                bottomY = tf.localPosition.y - cell_y / 2f;
                if (posY >= bottomY && posY <= topY)
                {
                    resultTf = tf;
                    targetHit = true;
                    break;
                }
            }
        }

        if (targetHit)
        {
            return resultTf;
        }
        else
        {
            return null;
        }

    }


    public void AddItem(Transform t,int imax)
    {
        if (mChild.Count >= imax) return;
        reuse_item_count = Mathf.Min(imax, dataNum);
        imax = reuse_item_count;
        if (mChild.Count >= imax) return;
        mChild.Add(t);
        startAxis = new Vector2(cell_x / 2f, -cell_y / 2f);//起始位置
        //初始化行列数
        int rows = 1, cols = 1;
        if (arrangeType == ArrangeType.Vertical) //垂直排列 则适应行数
        {
            rows = ConstraintCount;
            cols = (int)Mathf.Ceil((float)imax / (float)rows);
            extents = (float)(cols * cell_x) * 0.5f;

            datarows = rows;
            datacols = (int)Mathf.Ceil((float)dataNum / (float)rows);
        }
        else if (arrangeType == ArrangeType.Horizontal) //水平排列则适应列数
        {
            cols = ConstraintCount;
            rows = (int)Mathf.Ceil((float)imax / (float)cols);
            extents = (float)(rows * cell_y) * 0.5f;

            datacols = cols;
            datarows = (int)Mathf.Ceil((float)dataNum / (float)cols);
        }
        InitScrollValue(cols, rows);

        int x = 0, y = 0;//行列号
        int i = reuse_item_count -1;
        if (arrangeType == ArrangeType.Horizontal) { x = i / cols; y = i % cols; }
        else if (arrangeType == ArrangeType.Vertical) { x = i % rows; y = i / rows; }


        t.localPosition = new Vector2(startAxis.x + y * cell_x, startAxis.y - x * cell_y);

        if (i >= minIndex && i < dataNum)
        {
            setMyActive(t.gameObject, true);
            UpdateItem(t, i, i);
        }
        else
        {
            setMyActive(t.gameObject, false);
        }

        RefreshScrollViewSize(_dataNum);
    }
    //初始化各Item的坐标
    [ContextMenu("RePosition")]
    public void RePosition()
    {
        RePosition(true);
    }
    public void RePosition(bool scrollToSelected)
    {
        Awake();
        if (mScroll != null)
        {
            InitList();//初始化dataNum无关的大小尺寸参数，只执行一次
          
            InitValue();//
            if(scrollToSelected)UpdateSelect();
            WarpContentByRange();
        }
        hasReposition = true;

    }

    /// <summary>
    /// ScrollRect复位
    /// </summary>
   [ContextMenu("ResetPosition")]
   public void ResetPosition()
   {
       if (mScroll != null)
       {
         mScroll.StopMovement();
       }
       SetScrollRectPos(0);
   }

   public void SetSelectIndex(int index)
   {
       SetSelectIndex(index, true, true);
   }

   public void SetSelectIndex(int index, bool update,bool scrollToSelected)
   {
       _selectIndex = index;
       if (hasReposition)
       {
           if (scrollToSelected)
           {
               UpdateSelect();
           }
           if (update)
           {
                WarpContentByRange();
           }
       }
       else
       {
           RePosition(scrollToSelected);
       }

   }

   public void SetScrollRectPos (float ease)
   {
       if (mScroll != null)
       {
           mScroll.StopMovement();
       }
       this.easeValue = ease;
        if (hasReposition)
        {
            UpdateEase();
            WarpContentByRange();
        }
        else
        {
            RePosition();
        }
   }

   void UpdateSelect()
   {
       if (_selectIndex >= 0)
       {
           if (datacols <= 0 || datarows <= 0) return;
           Rect view = scrollRt.rect;
           int x = 0, y = 0;//行列号
           if (arrangeType == ArrangeType.Vertical) {
               x = _selectIndex / datarows; y = _selectIndex % datarows;
               float total = datacols * cell_x - view.width;
               if (total == 0)
               {
                   total = view.width;
               }
                if (this.ScrollToCenterOnSelected)
                {
                    this.easeValue = ( x * cell_x - (view.width - cell_y) / 2) / total;
                }
                else
                {
                    this.easeValue = x   * cell_x / total;
                }
           }
           else if (arrangeType == ArrangeType.Horizontal) {
               x = _selectIndex % datacols; y = _selectIndex / datacols;
               float total = datarows * cell_y - view.height;
               if (total == 0)
               {
                   total = view.height;
               }
                if (this.ScrollToCenterOnSelected)
                {
                     this.easeValue =  (y * cell_y - (view.height - cell_y) /2 ) / total;
                }else
                {
                    this.easeValue = y * cell_y / total;
                }
           }
       }
       UpdateEase();
   }

    private void Update()
    {
        if (canEase && EasyAnimation)
        {
            float cur = GetEase();
            float value = Mathf.Lerp(cur, this.easeValue, tempTime);
            SetEase(value);

            if(Mathf.Abs(value - this.easeValue) <= 0.005f)
            {
                canEase = false;
                tempTime = 0.0f;
                return;
            }

            // .. and increate the t interpolater
            tempTime += this.EasySpeed * Time.deltaTime;

            // now check if the interpolator has reached 1.0
            // and swap maximum and minimum so game object moves
            // in the opposite direction.
            if (tempTime > 1.0f)
            {
                float temp = maximum;
                maximum = minimum;
                minimum = temp;
                tempTime = 0.0f;
            }
        }
    }

    void UpdateEase()
   {
       if(this.easeValue < 0)
        {
            this.easeValue = 0;
        }else if(this.easeValue > 1)
        {
            this.easeValue = 1;
        }
        if (EasyAnimation)
        {
            canEase = true;
        }else
        {
            SetEase(this.easeValue);
        }
   }

    void SetEase(float value)
    {
           if (arrangeType == ArrangeType.Horizontal)
           {
               if (Mathf.Abs( 1 - value - CurrentScrollRect.verticalNormalizedPosition)>0.0001f)
                    CurrentScrollRect.verticalNormalizedPosition = 1 - value;
           }
           else
           {
               if (Mathf.Abs(value - CurrentScrollRect.horizontalNormalizedPosition) > 0.0001f)
                   CurrentScrollRect.horizontalNormalizedPosition = value;
           }
    }

    float GetEase()
    {
        if (arrangeType == ArrangeType.Horizontal)
        {
            return 1- CurrentScrollRect.verticalNormalizedPosition;
        }
        else
        {
            return CurrentScrollRect.horizontalNormalizedPosition ;
        }
    }


    /// <summary>
    /// 更新panel的尺寸
    /// </summary>
    /// <param name="pos"></param>
    void UpdateRectsize(Vector2 pos)
    {

        /*
        if (arrangeType == ArrangeType.Vertical)
        {
            
         //   if(mRTrans.rect.width<pos.x+cell_x)
            mRTrans.sizeDelta = new Vector2(pos.x + cell_x, ConstraintCount*cell_y);
        }
        else
        {
           
         //   if(mRTrans.rect.height<-pos.y+cell_y)
            mRTrans.sizeDelta = new Vector2(ConstraintCount * cell_x, -pos.y + cell_y);
        }
        */
    }
    //Vector2 calculatePos(Vector2 world,Vector2 target,Vector2 lcal)
    //{
    //    Vector2 temp = world - target;
    //    temp.x /= (target.x/lcal.x);
    //    temp.y /= (target.y/lcal.y);
        
    //    return temp;
    //}
    public int getRealIndex(Vector2 pos)//计算realindex
    {
        int x = (int)Mathf.Ceil(-pos.y / cell_y) - 1;//行号
        int y = (int)Mathf.Ceil(pos.x / cell_x) - 1;//列号

        int realIndex;
        if (arrangeType == ArrangeType.Horizontal)
        {
            realIndex = x * ConstraintCount + y;
        }
        else
        {
            realIndex = x + ConstraintCount * y;
        }

        return realIndex;

    }

    public int Index2RealIndex(int index)
    {
        Vector2 pos = mChild[index].localPosition;
        return getRealIndex(pos);
    }

    //瞬移
    void WarpContentByRange()
    {
        Vector3[] conners = new Vector3[4];
        //四角坐标  横着数
        conners[0] = new Vector3(-SR_size.x / 2f, SR_size.y / 2f, 0);
        conners[1] = new Vector3(SR_size.x / 2f, SR_size.y / 2f, 0);
        conners[2] = new Vector3(-SR_size.x / 2f, -SR_size.y / 2f, 0);
        conners[3] = new Vector3(SR_size.x / 2f, -SR_size.y / 2f, 0);
        for (int i = 0; i < 4; i++)
        {
            Vector3 temp = transform.parent.TransformPoint(conners[i]);
            conners[i].x = temp.x;
            conners[i].y = temp.y;
        }

        Vector3[] conner_local = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {

            conner_local[i] = mTrans.InverseTransformPoint(conners[i]);

        }

        if (mHorizontal)
        {
            float min = conner_local[0].x - cell_x;//显示区域
            float max = conner_local[3].x + cell_x;
            int showRow = reuse_item_count / ConstraintCount;
            int rowNum = Mathf.CeilToInt(dataNum * 1f / ConstraintCount);
            if(rowNum==0)
            {
                rowNum = 1;
            }
            int startRow = 0;
            for (int i = 0; i < rowNum; i++)
            {
                float rowX = cell_x * i ;
                if (rowX - cell_x / 2 < max && rowX + cell_x / 2 > min)
                {
                    startRow = i;
                    break;
                }
            }
            if((startRow+showRow)>rowNum)
            {
                startRow = rowNum - showRow;
            }
            if(startRow<0)
            {
                startRow = 0;
            }
            for (int i = 0; i < reuse_item_count; i++)
            {
                Transform temp = mChild[i];
                Vector2 pos = Vector2.zero;
                int tx = i % ConstraintCount;
                int ty = Mathf.FloorToInt(i *1f / ConstraintCount) + startRow;
                pos.x = (float)cell_x / 2 + ty * cell_x;
                pos.y = -(float)cell_y / 2 - cell_y * tx;
                temp.localPosition = pos;
                int realIndex = getRealIndex(pos);
                bool update = false;
                if ((realIndex >= minIndex && realIndex < dataNum))
                {
                    temp.localPosition = pos;
                    //设置Item内容
                    UpdateItem(temp, i, realIndex);
                    update = true;
                }
             
                pos = temp.localPosition;
                setMyActive(temp.gameObject,pos.x > min && pos.x < max && update);
            }
        }
        else
        {
            float min = conner_local[3].y - cell_y;//显示区域
            float max = conner_local[0].y + cell_y;
            int showRow = reuse_item_count / ConstraintCount;
            int rowNum = Mathf.CeilToInt(dataNum * 1f / ConstraintCount);
            if(rowNum == 0)
            {
                rowNum = 1;
            }
            int startRow = 0;
            for (int i = 0; i < rowNum; i++)
            {
                float rowY =  - cell_y * i;
                if (rowY - cell_y / 2 < max && rowY + cell_y / 2 > min)
                {
                    startRow = i;
                    break;
                }
            }
            if ((startRow + showRow) > rowNum)
            {
                startRow = rowNum - showRow;
            }
            if(startRow<0)
            {
                startRow = 0;
            }
            for (int i = 0; i < reuse_item_count; i++)
            {
                Transform temp = mChild[i];
                Vector2 pos = Vector2.zero;
                int tx = i % ConstraintCount;
                int ty = Mathf.FloorToInt(i *1f / ConstraintCount) + startRow;
                pos.x = cell_x / 2 + tx * cell_x;
                pos.y = -cell_y / 2 - cell_y * ty;
                temp.localPosition = pos;
                int realIndex = getRealIndex(pos);
                bool update = false;

                if ((realIndex >= minIndex && realIndex < dataNum))
                {
                    update = true;
                }

                setMyActive(temp.gameObject, pos.y > min && pos.y < max && update);

                if ((realIndex >= minIndex && realIndex < dataNum))
                {
                    temp.localPosition = pos;
                    //设置Item内容
                    UpdateItem(temp, i, realIndex);
                   
                }
            
            }
        }

        
    }

    /// <summary>
    /// 连续滚动
    /// </summary>
    /// <param name="value"></param>
    void WrapContent(Vector2 value)
    {
        Vector3[] conners = new Vector3[4];
        //四角坐标  横着数
        conners[0] = new Vector3(-SR_size.x / 2f, SR_size.y / 2f, 0);
        conners[1] = new Vector3(SR_size.x / 2f, SR_size.y / 2f, 0);
        conners[2] = new Vector3(-SR_size.x / 2f, -SR_size.y / 2f, 0);
        conners[3] = new Vector3(SR_size.x / 2f, -SR_size.y / 2f, 0);
        for (int i = 0; i < 4; i++)
        {
            Vector3 temp = transform.parent.TransformPoint(conners[i]);
            conners[i].x = temp.x;
            conners[i].y = temp.y;
        }
        Vector3[] conner_local = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            conner_local[i]=mTrans.InverseTransformPoint(conners[i]);
        }
        //计算ScrollRect的中心坐标 相对于this的坐标
        Vector2 center = (conner_local[3] + conner_local[0]) / 2f;


        bool needJump = false;  //跨度太大需要跳跃（非连续滚动）

        if (mHorizontal)
        {
            if (!canEase)
            {
                this.easeValue = value.x;
            }
            float min = conner_local[0].x - cell_x;//显示区域
            float max = conner_local[3].x + cell_x;
            for (int i = 0; i < reuse_item_count; i++)
            {
                Transform temp = mChild[i];
                float distance = temp.localPosition.x - center.x;
                Vector2 pos = temp.localPosition;
                bool update = false;
                if (distance <-extents)
                {
                    update = true;
                    pos.x += extents * 2f;

                    if (pos.x < conner_local[0].x)
                    {
                        //增加之后依然比边界小
                        needJump = true;
                        break;  //跳出循环
                    }
                }

                else if (distance > extents)
                {
                    update = true;
                    pos.x -= extents * 2f;

                    if (pos.x > conner_local[3].x)
                    {
                        //减小之后依然比边界大
                        needJump = true;
                        break;  //跳出循环
                    }

                }
                int realIndex = getRealIndex(pos);
                bool hasData = (realIndex >= minIndex && realIndex < dataNum);
                if (update)
                {

                    temp.localPosition = pos;

                    //设置Item内容
                    if (hasData) UpdateItem(temp, i, realIndex);
                }
                pos = temp.localPosition;
                setMyActive(temp.gameObject, pos.x > min && pos.x < max && hasData);
                    
            }
        }
        else 
        {
            if (!canEase)
            {
                this.easeValue = 1- value.y;
            }
            float min = conner_local[3].y - cell_y;//显示区域
            float max = conner_local[0].y + cell_y;

          
            for (int i = 0; i < reuse_item_count; i++)
            {
                Transform temp = mChild[i];
                float distance = temp.localPosition.y - center.y;
                Vector2 pos = temp.localPosition;
                 bool update = false;
                if (distance < -extents)
                {
                    update = true;
                    pos.y += extents * 2f;

                    if (pos.y < conner_local[0].y)
                    {
                        //增加之后依然比边界小
                        needJump = true;
                        break;  //跳出循环
                    }
                }

                else if (distance > extents)
                {
                    update = true;
                    pos.y -= extents * 2f;

                    if (pos.y > conner_local[3].y)
                    {
                        //减小之后依然比边界大
                        needJump = true;
                        break;  //跳出循环
                    }
                }
                int realIndex = getRealIndex(pos);
                bool hasData = (realIndex >= minIndex && realIndex < dataNum);

                bool isSHow = pos.y > min && pos.y < max && hasData;
                setMyActive(temp.gameObject, isSHow);

                if (update)
                {

                    temp.localPosition = pos;

                    //设置Item内容
                    if (hasData) UpdateItem(temp, i, realIndex);
                }
       
               
               
                    
               
            }

        }

        if (needJump)
        {
            WarpContentByRange();
        }


    }

    void UpdateItem(Transform item,int index,int realIndex)
    {
        if (onInitializeItem != null){
        
           onInitializeItem.Invoke(item.gameObject,index,realIndex);
           //onInitializeItem(item.gameObject, index, realIndex);
        }
    }

    void RefreshScrollViewSize (int dataNum)
    {
        if (ConstraintCount <= 0)
            ConstraintCount = 1;

        int horizontalLength = 0;
        int verticalLength = 0;
        if (arrangeType ==ArrangeType.Horizontal)
        {
            horizontalLength = ConstraintCount * cell_x;
            verticalLength = Mathf.CeilToInt((float)dataNum / ConstraintCount) * cell_y;
        }
        else
        {
            horizontalLength = Mathf.CeilToInt((float)dataNum / ConstraintCount) * cell_x;
            verticalLength = ConstraintCount * cell_y;
        }

        CurrentRTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, horizontalLength);
        CurrentRTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, verticalLength);
    }

    public void SetArrangeType(bool isVertical)
    {
        if (isVertical)
        {
            arrangeType = ArrangeType.Vertical;
        }
        else
        {
            arrangeType = ArrangeType.Horizontal;
        }
        
    }
    /*
    #if UNITY_EDITOR
        void OnValidate()
        {
            if (!Application.isPlaying && mScroll != null)
            this.RePosition();
        }
 
    #endif
     * */
    [Serializable]
    public class ItemUpdate : UnityEvent<GameObject, int, int>
    {

    }

    private void setMyActive(GameObject obj, bool isActive)
    {
        if (null != obj)
        {
            if (obj.activeSelf != isActive)
            {
                obj.SetActive(isActive);
            }
        }
    }
}



