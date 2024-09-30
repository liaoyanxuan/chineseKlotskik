using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockObj : MonoBehaviour
{
    public static bool islocking = false;

    [SerializeField]
    public SpriteRenderer image;
    [SerializeField]
    public BlockObjType blockType = BlockObjType.None;

    [SerializeField]
    public int blockWidth = 1;
    [SerializeField]
    public int blockHeight = 1;
    [SerializeField]
    private int blockRange = 1;
    [SerializeField]
    private bool horizontal = false;
    [SerializeField]
    private bool blockSpecial = false;
   
    [SerializeField]
    private GameObject fadeObj;
    [SerializeField]
    private GameObject hintObj;

    [SerializeField]
    private List<GameObject> boxs = new List<GameObject>();


    public char charName;
    public int blockStyle;

    public int width;
    public int height;
    public bool blockMain;
    public bool isHitSpecial = false;
    public int verticalIndex = 0;
    public int horizontalIndex = 0;
    public Vector3 savePosition;

    public int GetIntPositionY
    {
        get
        {
            return Mathf.RoundToInt(transform.localPosition.y);
        }
    }
    public int GetIntPositionX
    {
        get
        {
            return Mathf.RoundToInt(transform.localPosition.x);
        }
    }
    public int GetBlockRange { get { return blockRange; } }
    public bool GetHorizontal { get { return horizontal; } }
    public bool GetBlockSpecial { get { return blockSpecial; } }

    private Vector3 winTarget = new Vector3(8, 3, 0);
    private Vector3 velocity = Vector3.zero;
    private float smoothSpeed =0.25f;
    private bool isUpdate = false;

    private Vector3 originPosition = Vector3.zero;


    [SerializeField] private float moveSpeed = 100;

    private Vector3 mPMousePos;
    private Vector3 mPPos;
    private AudioSource mAudioSource;
    private Rigidbody2D rb;

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    /*
     * Dynamic behaviour causes the Rigidbody2D to react to gravity and applied forces including contacts with other dynamic or Kinematic Rigidbody2D.
     * Kinematic behaviour stops the Rigidbody2D from reacting to gravity or applied forces including contacts with other Kinematic or Static Rigidbody2D.
     * Static behaviour stops the Rigidbody2D from reacting to gravity or applied forces including contacts with any other Rigidbody2D.
     * 
     * Dynamic Rigidbody2D被設計用來製作在物理模擬下會移動的物體。它會與所有類型的Rigidbody2D進行碰撞，是最常用的Rigidbody2D類型、默認的類型，同時也是最耗費性能的類型。
     *  文檔中粗體部分的意思就是千萬不要使用Transform組件來設置Dynamic Rigidbody2D的position和rotation。
     *  如果想要移動或旋轉它，可以使用上面提到的方式，通過Rigidbody2D的接口來完成。
     * 
     * 文檔中粗體部分的意思是Kinematic Rigidbody2D對系統資源的要求比Dynamic Rigidbody2D更低（所以更有效率）。Kinematic Rigidbody2D被設計用來通過Rigidbody2D.MovePosition或Rigidbody2D.MoveRotation來進行重定位。
     *   對於Kinematic Rigidbody2D，velocity屬性對它依舊有效，只不過施加力和重力都不會對velocity造成影響。
     *   Kinematic Rigidbody2D僅僅只會與Dynamic的Rigidbody2D發生碰撞（在不勾選Use Full Kinematic Contacts的情況下），它在碰撞行爲上類似於Static Rigidbody2D，可以理解爲具有無限質量、無法被撼動（不能通過力或碰撞改變速度，但是可以設置其速度和位置、旋轉）的剛體。

     * Static Rigidbody2D被設計用來製作在物理模擬下不會移動的物體。它在表現上可以理解爲一個具有無限質量、不可移動的物體。此時velocity、AddForce、gravity、MovePosition、MoveRotation都是不可用的。
     *   文檔中粗體部分意思是Static Rigidbody2D對資源最不敏感（對性能要求低），Static Rigidbody2D僅僅會與Dynamic Rigidbody2D發生碰撞。兩個Static Rigidbody2D之間也不會發生碰撞，因爲他們本來就被設計成不可移動的。
    */
    private Vector3 exitPos = new Vector3(1f, 0f, 0f);
    private void OnCollisionEnter2D(Collision2D other)
    {
       // Debug.Log("BlockObj-->OnCollisionEnter2D");

        if (CompareTag("Main Block") && other.gameObject.CompareTag("Exit"))
        {
            float exitDiatance = Vector3.Distance(exitPos, transform.localPosition);
             Debug.Log("Distance:" + exitDiatance);

            if (exitDiatance < 0.08f)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                PlayingManager.instance.WinGame();
            }
        }

    }

    private bool isDowned=false;


    /*
      OnMouseDown is called when the user has pressed the mouse button while over the Collider.
      This event is sent to all scripts of the GameObject with Collider. Scripts of the parent or child objects do not receive this event.
     */
    private void OnMouseDown()
    {
        if (IsPointerOverUIObject(Input.mousePosition.x, Input.mousePosition.y))
        {  //判断鼠标或者手指是否点击在UI上
            return;
        }
            //当前触摸在UI上
            if (Camera.main != null && fadeObj.activeSelf == false && BoosterManager.instance.IsMoveBeginGame && islocking==false)
        {
            islocking = true;
            isDowned = true;
            mPPos = transform.localPosition;
          //  Debug.Log("BlockObj-->OnMouseDown "+ BoosterManager.instance.IsMoveBeginGame);
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 0f;
            mPMousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            // mPMousePos -= transform.localPosition;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

    }

    private void OnMouseDrag()
    {
        if (isDowned==true && Camera.main != null && fadeObj.activeSelf == false && BoosterManager.instance.IsMoveBeginGame)  // 
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            //  Debug.Log("BlockObj-->OnMouseDrag");
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 0f;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            // rb.velocity = (mouseWorldPos - mPMousePos - transform.localPosition) * 50;
            rb.velocity = (mouseWorldPos - mPMousePos) * moveSpeed;  //表示速度
            //Debug.Log("rb.velocity:"+ rb.velocity);
            mPMousePos = mouseWorldPos;
        }

    }

    private void OnMouseUp()   //归位到最靠近的格子
    {
       
        if (isDowned == true && Camera.main != null && fadeObj.activeSelf == false && BoosterManager.instance.IsMoveBeginGame)  // 
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            Vector2 pos = transform.localPosition;
            float minDistance = float.PositiveInfinity;
            Vector2 minPos = pos;

            foreach (Vector2 gridPos in PlayingManager.instance.GetGridPos())
            {
                float distance = Vector2.Distance(pos, gridPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minPos = gridPos;
                }
            }

            /*
                    if (BackgroundMusic.Instance != null && BackgroundMusic.Instance.GetComponent<AudioSource>().isPlaying)
                    {
                        mAudioSource.PlayOneShot(mAudioSource.clip);
                    }
            */
            if (PlayingManager.instance.checkIsInHitOrNormalMove(minPos))
            {
                // Debug.Log("Distance:" + Vector3.Distance(minPos, mPPos));
                if (Vector3.Distance(minPos, mPPos) > 0.9f)  //增加步数
                {
                    // PlayArea.Instance.AddStep();
                    PlayingManager.instance.AddMove(this,gameObject, mPPos, minPos);
                    transform.localPosition = minPos;
                 
                }
                else 
                {
                    transform.localPosition = mPPos;
                }
            }
            else
            {
                transform.localPosition = mPPos;
            }
        }

        savePosition = transform.localPosition;
        isDowned = false;
        islocking = false;
    }

    private bool IsPointerOverUIObject(float x, float y)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(x, y);

        List<RaycastResult> results = new List<RaycastResult>();
        if (EventSystem.current != null)
        {
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        }

        return results.Count > 1;
    }
    /*
    //胜利逃逸
    private void Update()
    {
        if (!isUpdate) return;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, winTarget, ref velocity, smoothSpeed);
    }
    */
    public void SetUp(Vector3 position,int index,bool move =true)
    {
        isUpdate = false;
        islocking = false;
        originPosition = position;
        Vector3 newPosition = originPosition;
        newPosition.x += 10;
        transform.localPosition =(move)? newPosition:originPosition;
        savePosition = originPosition;
        if (move)
        {
            LeanTween.moveLocal(gameObject, originPosition, GameManager.speedMoveBlock).setDelay(0.2f*index).setOnComplete(completed=> {
                UpdateBoxGrid();
                SoundManager.instance.FirstBlockSound();
            }).setEaseOutQuint();
        }
        VisibleImageObj(true);
        VisibleHintObj(false);
        VisibleFade(false);
    }


    public void DeActiveMoveEffect()
    {
        isUpdate = false;
    }
    public void UpdateBoxGrid2()
    {
        int startX = GetIntPositionX;

        int startY = GetIntPositionY;
        for (int i = 0; i < boxs.Count; i++)
        {
           
            if (horizontal)
            {
                int x = startX + i;
                PlayingManager.instance.GridInGame[startY].grids[x] = boxs[i];
                boxs[i].transform.name = string.Format("Grid_{0}_{1}", startY, x);
            }
            else
            {
                int y = startY + i;
                PlayingManager.instance.GridInGame[y].grids[startX] = boxs[i];
                boxs[i].transform.name = string.Format("Grid_{0}_{1}", y, startX);
            }
         
        }
    }

    public void UpdateBoxGrid()
    {
        
    }


    public Vector3 GetHintPosition(int moveRow,int moveCol)
    {
        Vector3 position = transform.localPosition;
        position.x += moveCol;
        position.y += moveRow;
        return position;

    }

    public void WinGame()
    {
        isUpdate = true;
        SoundManager.instance.xiuflashSound();
    }




    /// <summary>
    /// Limit position can move
    /// </summary>
    public int PointCanMoveInLine(bool max)
    {
        int startX = GetIntPositionX;
        int startY = GetIntPositionY;
        int y = GetIntPositionY;
        int width = GameManager.gridWidth;
        int height = GameManager.gridHeight;
        int limit = 0;
        if (max)
        {

            if (horizontal)
            {
                startX += blockRange;  //起点加长度
                int count = 0;
                for (int i = startX; i < width; i++)
                {
                    if (PlayingManager.instance.GridInGame[y].grids[i] != null)
                    {
                        return GetIntPositionX + count;
                    }
                    count++;//格子为空则可前进
                }
                limit = GetIntPositionX + count; //最大前进范围
            }
            else
            {
                startY += blockRange;
                int count = 0;
                for (int i = startY; i < height; i++)
                {
                    if (PlayingManager.instance.GridInGame[i].grids[startX] != null)
                    {
                        return GetIntPositionY + count;
                    }
                    count++;
                }
               
                limit = GetIntPositionY + count;
            }
        }
        else
        {
            if (horizontal)
            {
                for (int i = startX - 1; i >= 0; i--)
                {
                    if (PlayingManager.instance.GridInGame[y].grids[i] != null)
                    {
                        return i + 1;
                    }
                }
                limit = 0;
            }
            else
            {
                for (int i = startY - 1; i >= 0; i--)
                {
                    if (PlayingManager.instance.GridInGame[i].grids[startX] != null)
                    {
                        return i + 1;
                    }
                }
                limit = 0;
            }
        }

        return limit;
    }


    /// <summary>
    /// Limit position can move
    /// </summary>
    public int PointCanMoveInLine_X(bool max)
    {
        int startX = GetIntPositionX;
        int y = GetIntPositionY;
        int width = GameManager.gridWidth;
        int limit = 0;
        if (max)
        {
           
            startX += blockWidth;  //起点加长度
            int count = 0;
            for (int i = startX; i < width; i++)
            {
                if (PlayingManager.instance.GridInGame[y].grids[i] != null)
                {
                    return GetIntPositionX + count;
                }
                count++;//格子为空则可前进
            }
            limit = GetIntPositionX + count; //最大前进范围
           
        }
        else
        {
            for (int i = startX - 1; i >= 0; i--)
            {
                if (PlayingManager.instance.GridInGame[y].grids[i] != null)
                {
                    return i + 1;
                }
            }
            limit = 0;
        }

        return limit;
    }


    /// <summary>
    /// Limit position can move
    /// </summary>
    public int PointCanMoveInLine_Y(bool max)
    {
        int startX = GetIntPositionX;
        int startY = GetIntPositionY;
        int height = GameManager.gridHeight;
        int limit = 0;
        if (max)
        {
                startY += blockHeight;
                int count = 0;
                for (int i = startY; i < height; i++)
                {
                    if (PlayingManager.instance.GridInGame[i].grids[startX] != null)
                    {
                        return GetIntPositionY + count;
                    }
                    count++;
                }

                limit = GetIntPositionY + count;
            
        }
        else
        {

                for (int i = startY - 1; i >= 0; i--)
                {
                    if (PlayingManager.instance.GridInGame[i].grids[startX] != null)
                    {
                        return i + 1;
                    }
                }
                limit = 0;
            
        }

        return limit;
    }

    public void VisibleImageObj(bool visible)
    {
      
        image.gameObject.SetActive(visible);
    }
    public void VisibleHintObj(bool visible)
    {
        hintObj.SetActive(visible);
    }

    public void VisibleFade(bool visible)
    {
        fadeObj.SetActive(visible);
      
    }

  
}

public enum BlockObjType
{
    None=0,
    Single=1,
    Horizontal = 2,
    Vertical=3,
    BigBlock=4,
}
