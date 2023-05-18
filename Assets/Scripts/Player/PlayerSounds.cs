using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static PlayerSounds instance {get; private set; }
    public AudioSource _source;
    
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        _source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound, float volume)
    {
        _source.volume = volume;
        _source.PlayOneShot(sound);
    }
}
