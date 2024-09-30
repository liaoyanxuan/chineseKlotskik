using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownWidget : MonoBehaviour
{

    [SerializeField]
    private CountTimer counterTimer;

    [SerializeField]
    private Text counterTxt;


    [SerializeField]
    private Button confrimBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   
    public void startCountDown(int tryTimes)
    {
        counterTimer.repeatCount = 60 * tryTimes;

        confrimBtn.gameObject.SetActive(false);
        this.gameObject.SetActive(true);

        counterTimer.reset();
        counterTimer.resume();

        counterTxt.text = counterTimer.repeatCount.ToString() + " 秒后可认证";
    }

    public void CounterComplete()
    {
        counterTimer.stop();
        this.gameObject.SetActive(false);
        confrimBtn.gameObject.SetActive(true);
        confrimBtn.interactable = true;
    }

    public void CounterInterval()
    {
        counterTxt.text = (counterTimer.repeatCount - counterTimer.currentCount).ToString() + " 秒后可认证";
    }
}
