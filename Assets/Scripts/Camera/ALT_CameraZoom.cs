using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALT_CameraZoom : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.0f;
    

    public GameObject Player;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        Vector3 offset = new Vector3(playerMovement.rb.velocity.x / 14f, 1, -5);
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
