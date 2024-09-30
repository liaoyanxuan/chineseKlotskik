using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using UnityEngine;

public class AgeTipPanelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onClose()
    {
        UIController.Instance.ageTipsPanel.Deactivate();
    }
   
}
