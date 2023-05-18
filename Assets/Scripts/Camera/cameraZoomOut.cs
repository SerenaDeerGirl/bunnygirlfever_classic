using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraZoomOut : MonoBehaviour
{
    public GameObject camera;
    public float zoomAmount;
    public bool changeOffset;
    public bool resetOffset;
    public float offsetX;
    public float offsetY; 

    private ALT2P_CameraZoom cameraScript;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraScript = camera.GetComponent<ALT2P_CameraZoom>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("YAYYYY");
        cameraScript.size = zoomAmount;
        if(resetOffset)
        {
            cameraScript.isFixed = false;
            cameraScript.offset.x = 0;
            cameraScript.offset.y = 1;
        }
        else if(changeOffset)
        {
            cameraScript.isFixed = true;
            cameraScript.offset.x = offsetX;
            cameraScript.offset.y = offsetY;
        }
    }
}
