using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotAgreeBtnScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(notAgreeClick);
    }


    void notAgreeClick()
    {
      //  Debug.Log("notAgreeClick");
        Application.Quit();
       
    }
}
