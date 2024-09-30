using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopUpFrameWork;
using UnityEngine.UI;
using TMPro;

public class PopupADThanks : Popup
{
    [SerializeField]
    private TextMeshProUGUI processTxt;

    [SerializeField]
    private TextMeshProUGUI thanksTxt;

    public void setProcessTxt(string process)
    {
        processTxt.text = process;
    }

    public void setThanksTxt(string thanks)
    {
        thanksTxt.text = thanks;
    }
}