using System.Collections;
using System.Collections.Generic;
using sw.util;
using UnityEngine;
using UnityEngine.UI;

public class buttonBannerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        if (AndroidUtil.isCanShareOther() == false)
        {
            this.GetComponent<Image>().enabled = false;
        }
    }

   
}
