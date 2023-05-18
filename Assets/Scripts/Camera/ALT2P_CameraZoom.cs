using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALT2P_CameraZoom : MonoBehaviour
{
    public Transform target;
    public Camera m_OrthographicCamera;
    public Vector3 offset = new Vector3(0, 1, -5);

    public float smoothSpeed = 0.0f;
    public bool isFixed;
    public float fixedX;
    public float fixedY;
    public float size;
    private float startingSize;

    public GameObject Player;
    private PlayerMovement playerMovement;
    private PlayerMovementP2 playerMovementP2;
    private PlayerCheck playerCheck;

    private void Start()
    {
        playerCheck = Player.GetComponent<PlayerCheck>();
        if(playerCheck.player == 1)
        {
            playerMovement = Player.GetComponent<PlayerMovement>();
        }
        else if(playerCheck.player == 2)
        {
            playerMovementP2 = Player.GetComponent<PlayerMovementP2>();
        }
        
    }

    private void FixedUpdate()
    {
        startingSize = m_OrthographicCamera.orthographicSize;
        m_OrthographicCamera.orthographicSize = Mathf.SmoothStep(startingSize, size, 0.19f);
        if(isFixed)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            if(playerCheck.player == 1)
            {
                offset = new Vector3(playerMovement.rb.velocity.x / 14f, 1, -5);
            }
            else if(playerCheck.player == 2)
            {
                offset = new Vector3(playerMovementP2.rb.velocity.x / 14f, 1, -5);
            }
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        
    }
}
