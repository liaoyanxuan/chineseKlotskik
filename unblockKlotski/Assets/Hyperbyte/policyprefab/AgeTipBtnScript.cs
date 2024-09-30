using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using UnityEngine;
using UnityEngine.UI;

public class AgeTipBtnScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() => {

            UIController.Instance.ageTipsPanel.Activate();
        });
    }


   
}
