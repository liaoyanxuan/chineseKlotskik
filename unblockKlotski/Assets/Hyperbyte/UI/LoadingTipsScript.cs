using Hyperbyte;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTipsScript : MonoBehaviour
{
    private Coroutine coroutine;
 

    public void ShowLoadingTip()
    {
        this.gameObject.Activate();
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = TimerSchedule.Schedule(this, 10f, () =>
        {
            this.gameObject.Deactivate();
            coroutine = null;
        }
        );

        
    }


    public void HideLoadingTip()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        this.gameObject.Deactivate();
    }



}
