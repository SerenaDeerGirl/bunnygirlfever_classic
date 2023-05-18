using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public GameObject Player;
    public float springAmount;
    private PlayerMovement playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = Player.GetComponent<PlayerMovement>();
    }

    async void OnTriggerEnter2D(Collider2D col)
    {
        playerMovement.extraVVelocity = springAmount;
        for(int i = 0; i < 180; i++) 
        {
            await Task.Delay(1);
        }
    }
}
