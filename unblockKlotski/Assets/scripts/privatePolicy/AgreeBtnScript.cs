using System;
using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using sw.game.evt;
using UnityEngine;
using UnityEngine.UI;

public class AgreeBtnScript : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(agreeClick);
    }


    void agreeClick()
    {
      

        UIController.Instance.privacyPanel.Deactivate();

      

    }
}
