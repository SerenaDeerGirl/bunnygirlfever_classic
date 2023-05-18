using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityBar : MonoBehaviour
{
    public GameObject Player;
    private PlayerMovement playerMovement;
    public Image velocityBar;

    private void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    // Healthbar fill max is 1
    void Update()
    {
        velocityBar.fillAmount = Math.Abs(playerMovement.rb.velocity.x) / 30f;
    }
}
