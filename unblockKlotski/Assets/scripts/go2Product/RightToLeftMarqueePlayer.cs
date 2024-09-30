using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class RightToLeftMarqueePlayer : MonoBehaviour
{
    public RectTransform content;   // 存放图片的滚动区域
    public RectTransform targetContent; //目标nei'ron

    private float scrollSpeed = 50f; // 滚动速度
  
  

    private bool isScrolling = false;

    private void Start()
    {

        // 启动跑马灯协程
        StartCoroutine(MarqueeCoroutine());
    }

    private IEnumerator MarqueeCoroutine()
    {
        //等待2秒
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // 移动图片至屏幕左侧
            isScrolling = true;
           
            // 播放图片向右滚动动画
            while (content.anchoredPosition.x > -content.rect.width)
            {
                content.anchoredPosition -= Vector2.right * scrollSpeed * Time.deltaTime;
                yield return null;
            }

            // 图片移动出屏幕后，将其移到屏幕左侧开始位置，并更新显示图片

            content.anchoredPosition = new Vector2(content.rect.width, 0f);

            //校验 targetContent此时应该在0附近,如果偏移超过5个像素，则强制纠正
            Debug.Log("targetContent.anchoredPosition.x:" + targetContent.anchoredPosition.x);
            if (Mathf.Abs(targetContent.anchoredPosition.x) >= 2)
            {
                Debug.Log("targetContent-RePosition!!!:" + targetContent.anchoredPosition.x);
                targetContent.anchoredPosition= new Vector2(0f, 0f);
            }
          

            isScrolling = false;
        }
    }
}
