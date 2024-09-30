using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServiceprotoclBtnScript : MonoBehaviour
{
    [SerializeField]
    private GameObject scrollViewServiceprotocol;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(clickHandler);
    }

    // Update is called once per frame
    void clickHandler()
    {
        scrollViewServiceprotocol.SetActive(true);
    }
}
