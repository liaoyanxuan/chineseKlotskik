using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAreaScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_IOS
    this.gameObject.SetActive(false);
#else
        this.gameObject.SetActive(false);
#endif


    }

    // Update is called once per frame

}
