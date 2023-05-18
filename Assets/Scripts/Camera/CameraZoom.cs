using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera m_OrthographicCamera;
    float m_ViewPositionX, m_ViewPositionY, m_ViewWidth, m_ViewHeight;
    float cameraSize = 0.0f;
    public GameObject FirstOBJ;
    public GameObject SecondOBJ;
    
    // Start is called before the first frame update
    void Start()
    {
        m_ViewPositionX = 0;
        m_ViewPositionY = 0;

        //This sets the Camera view rectangle to be smaller so you can compare the orthographic view of this Camera with the perspective view of the Main Camera
        m_ViewWidth = 1f;
        m_ViewHeight = 1f;
        m_OrthographicCamera.enabled = true;

        //If the Camera exists in the inspector, enable orthographic mode and change the size
        if (m_OrthographicCamera)
        {
            //This enables the orthographic mode
            m_OrthographicCamera.orthographic = true;
            //Set the size of the viewing volume you'd like the orthographic Camera to pick up (5)
            m_OrthographicCamera.orthographicSize = 3.5f;
            //Set the orthographic Camera Viewport size and position
            m_OrthographicCamera.rect = new Rect(m_ViewPositionX, m_ViewPositionY, m_ViewWidth, m_ViewHeight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rb = FirstOBJ.GetComponent<Rigidbody2D>();
        Vector3 v3Velocity = rb.velocity;
        Vector3 v3Position = rb.position; 
        SecondOBJ.transform.position = new Vector3(v3Position.x + (v3Velocity.x * 0.009f), v3Position.y + 1f, -5); // camera code LOL

        m_OrthographicCamera.orthographicSize = 3.5f + cameraSize;
        if(Input.GetButtonDown("ZoomOut"))
        {
            cameraSize = cameraSize - 0.5f;
        }
        else if(Input.GetButtonDown("ZoomIn"))
        {
            cameraSize = cameraSize + 0.5f;
        }
    }
}
