using UnityEngine.Audio;
using UnityEngine;
using System;

/**
*   AUDIO MANAGER component
*   A list of sounds that we can add or easily remove from/to.
*   manages the auido fies.
*/ 
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;//A list of all sound components used

    // Awake is called before the first frame update and before Start()
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    //starts the playback of sound with given name
    public void Play(string name, bool loop)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);//look up array
        s.source.Play();
        if(loop) s.source.loop = true;
    }
}
