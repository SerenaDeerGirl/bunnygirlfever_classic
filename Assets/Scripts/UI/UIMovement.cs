using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMovement : MonoBehaviour
{
    // Components
    RectTransform RectTransform;
    public GameObject Player;
    private PlayerMovement playerMovement;

    // Script Properties
    public float initX, initY;
    
    // Start is called before the first frame update
    void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Math.Abs(playerMovement.rb.velocity.y) > 7f)
        {
            if(playerMovement.rb.velocity.y > 0.0f)
            {
                RectTransform.anchoredPosition = new Vector3(initX, initY + ((playerMovement.rb.velocity.y - 7f) / -10f), 0.0f);
            }
            else
            {
                RectTransform.anchoredPosition = new Vector3(initX, initY + ((playerMovement.rb.velocity.y + 7f) / -10f), 0.0f);
            }
        }
        else
        {
        RectTransform.anchoredPosition = new Vector3(initX, initY, 0.0f);
        }
    }
}
