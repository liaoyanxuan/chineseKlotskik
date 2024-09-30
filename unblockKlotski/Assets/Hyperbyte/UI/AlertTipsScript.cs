using Hyperbyte;
using Hyperbyte.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertTipsScript : MonoBehaviour
{
    [SerializeField]
    private Text txtTips;
    // Start is called before the first frame update

    [SerializeField]
    private Button NoButton;

    [SerializeField]
    private TextMeshProUGUI YesButtonDesc;

    [SerializeField]
    private TextMeshProUGUI NoButtonDesc;

   

    private Action _yesHandle;
    private Action _noHandle;

    // Update is called once per frame
    public void showTips(string tips)
    {
        txtTips.text = tips;
        YesButtonDesc.text = LocalizationManager.Instance.GetTextWithTag("txtOK");
        NoButton.gameObject.SetActive(false);
        _yesHandle = null;
        _noHandle = null;
    }

    public void showTipsWithNo(string tips, string yesDesc, string NoDesc, Action yesHandle, Action noHandle)
    {
        txtTips.text = tips;
        YesButtonDesc.text = yesDesc;
        NoButtonDesc.text = NoDesc;
        _yesHandle = yesHandle;
        _noHandle = noHandle;

        NoButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Ok button click listener.
    /// </summary>
    public void OnOkButtonPressed()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            gameObject.Deactivate();

            if (_yesHandle != null)
            {
                _yesHandle.Invoke();
                _yesHandle = null;
            }
        }
    }

    public void OnNoButtonPressed()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            gameObject.Deactivate();

            if (_noHandle != null)
            {
                _noHandle.Invoke();
                _noHandle = null;
            }
        }
    }
}