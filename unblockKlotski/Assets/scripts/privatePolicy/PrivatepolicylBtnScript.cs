using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivatepolicylBtnScript : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollViewPrivatepolicy;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(clickHandler);
    }

    // Update is called once per frame
    void clickHandler()
    {
        scrollViewPrivatepolicy.SetActive(true);
    }
}
