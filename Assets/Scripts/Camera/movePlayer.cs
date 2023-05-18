using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlayer : MonoBehaviour
{
    public GameObject Player;
    public float desiredX;
    public float desiredY;

    void OnTriggerEnter2D(Collider2D col)
    {
        Player.transform.position = new Vector3(desiredX, desiredY, 0f);
    }
}
