using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class CameraSize : MonoBehaviour
{


    [SerializeField]
    private Camera cameraObj;
 
    [SerializeField]
    private float sizeCamera = 1;

    private float aspect = 1;
    private Vector3 originPosition;
    private Vector3 originBGPosition;
    private float sizeCameraInBoard10x12 = 0;



    private void Update()
    {
 
        cameraObj = Camera.main;
        Initialize();
 

    }


    private void Awake()
    {
        
        if (RatioResolution.GetResolution() <= 1.5f)
        {
            Vector3 position = transform.position;
            position.y += (0.4f/1.33f)* RatioResolution.GetResolution();
            transform.position = position;
        }
        
    }

    private void Initialize()
    {
        aspect = (float)Screen.height / (float)Screen.width;
        aspect = (float)Math.Round(aspect, 2);

       // Debug.Log("CameraSize-->aspect:" + aspect+ " sizeCamera:" + sizeCamera);

        float size = sizeCamera;
        if (aspect >1.1f && aspect <= 1.5f)
        {
            size = 6/ aspect;
        }
        else if(aspect < 1.1f)
        {
            size = 6 / aspect;
            size += 1.1f;
        }


        cameraObj.orthographicSize = aspect * size + sizeCameraInBoard10x12;

       // Debug.Log("CameraSize-->size:" + size + " orthographicSize:" + cameraObj.orthographicSize);
    }

}
