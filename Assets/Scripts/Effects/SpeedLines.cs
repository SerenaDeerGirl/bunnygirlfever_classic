using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public GameObject Player;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        var emission = particleSystem.emission;
        var direction = particleSystem.shape;
        
        if(Math.Abs(playerMovement.rb.velocity.x) > 21)
        {
            emission.enabled = true;
            direction.rotation = new Vector3(0f, -90 * playerMovement.dirX, 0f);
            direction.position = new Vector3(10.322f * playerMovement.dirX, 0, -6.1526e-07f);
            particleSystem.startSpeed = 40f + (2f * (playerMovement.rb.velocity.x - 21));

        }
        else
        {
            emission.enabled = false;
        }
    }
}
