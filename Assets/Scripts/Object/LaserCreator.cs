using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LaserCreator : MonoBehaviour
{
    public Sprite laserSprite;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    async void Update()
    {
        for(int i = 0; i < 180; i++) 
        {
            await Task.Delay(1);
        }
        GameObject laser = new GameObject();
        laser.name = "ouchie cause r";
        laser.AddComponent<Rigidbody2D>();
        laser.AddComponent<BoxCollider2D>();
        laser.AddComponent<LaserObject>();
        laser.AddComponent<SpriteRenderer>();

        var spriterenderer = laser.GetComponent("SpriteRenderer");
    }
}
