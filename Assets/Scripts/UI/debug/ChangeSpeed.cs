using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour
{
    public InputField input;
    public GameObject player;
    public Button button;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        button.onClick.AddListener(TaskOnClick);
    }
    
    void TaskOnClick()
    {
        playerMovement.maxSpeed = float.Parse(input.text);
        input.text = "Changed to " + input.text;
    }
}
