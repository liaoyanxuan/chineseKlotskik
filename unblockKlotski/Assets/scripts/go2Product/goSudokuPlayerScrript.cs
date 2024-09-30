using System.Collections;
using System.Collections.Generic;
using sw.util;
using UnityEngine;
using UnityEngine.UI;

public class goSudokuPlayerScrript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(goClick);

        if(AndroidUtil.isCanShareOther() == false)
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void goClick()
    {
#if UNITY_IOS
        iOSUtil.goToProductPage("1489801991");
#endif


#if UNITY_ANDROID
        string taptapUrl = "https://www.taptap.com/app/216181";
        Application.OpenURL(taptapUrl);
#endif
	}
}
