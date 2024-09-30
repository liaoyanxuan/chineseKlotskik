using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using sw.util;
using UnityEngine;
using UnityEngine.UI;

public class ServiceAndPrivacyShowBtnScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(serviceAndPrivacyShowClick);

        if (AndroidUtil.iSShowPrivacyPolicy == 1 || AndroidUtil.iSShowIndependentPrivacyPolicy == 1)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }


    void serviceAndPrivacyShowClick()
    {
        if (AndroidUtil.iSShowPrivacyPolicy == 1)
        {
            UIController.Instance.privacyPanel.Activate();
        }
        else if (AndroidUtil.iSShowIndependentPrivacyPolicy == 1)
        {
            AndroidUtil.showIndependentPrivacy();
        }
    }
}
