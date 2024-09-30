using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnCloseScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(ClickHandler);
    }

   

    void ClickHandler()
    {
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
