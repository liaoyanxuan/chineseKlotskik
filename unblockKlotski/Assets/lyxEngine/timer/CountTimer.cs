using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器
/// </summary>
public class CountTimer : MonoBehaviour
{

    // 延迟时间(秒)
    public float delay = 0;
    // 间隔时间(秒)
    public float interval = 1;
    // 重复次数
    public int repeatCount = 0;
   
    // 自动销毁
    public bool autoDestory = true;
    // 当前时间,累加
    public float currentTime = 0;
    // 当前次数
    public int currentCount = 0;
    // 计时间隔
    public UnityEvent onIntervalEvent;
    // 计时完成
    public UnityEvent onCompleteEvent;
    // 回调事件代理
    public delegate void TimerCallback(CountTimer timer);
    // 上一次间隔时间
    private float lastTime = 0;
    // 计时间隔
    private TimerCallback onIntervalCall;
    // 计时结束
    private TimerCallback onCompleteCall;


    void Awake() 
    {
        enabled = false;
    }


    private void FixedUpdate()
    {
        if (!enabled) return;
        addInterval(Time.deltaTime);
    }

    /// <summary> 增加间隔时间 </summary>
    private void addInterval(float deltaTime)
    {
        currentTime += deltaTime;
        if (currentTime < delay) return;
        if (currentTime - lastTime >= interval)
        {
            currentCount++;
            lastTime = currentTime;
            if (repeatCount <= 0)
            {
                // 无限重复
                if (currentCount == int.MaxValue) reset();
                if (onIntervalCall != null) onIntervalCall(this);
                if (onIntervalEvent != null) onIntervalEvent.Invoke();
            }
            else
            {
                if (currentCount < repeatCount)
                {
                    //计时间隔
                    if (onIntervalCall != null) onIntervalCall(this);
                    if (onIntervalEvent != null) onIntervalEvent.Invoke();
                }
                else
                {
                    //计时结束
                    stop();
                    if (onCompleteCall != null) onCompleteCall(this);
                    if (onCompleteEvent != null) onCompleteEvent.Invoke();
                    if (autoDestory && !enabled) Destroy(this);
                }
            }
        }
    }

    /// <summary> 开始/继续计时 </summary>
    public void resume()
    {
        
        enabled  = true;
    }

   
    /// <summary> 暂停计时 </summary>
    public void stop()
    {
        
        enabled  = false;
    }

    /// <summary> 停止Timer并重置数据 </summary>
    public void reset()
    {
        lastTime = currentTime = currentCount = 0;
    }



}
