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
    public static AudioManager instance;//singleton

    // Awake is called before the first frame update and before Start()
    void Awake()
    {
        //SINGLETON
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);//to ensure audio plays through scenes

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //starts the playback of sound with given name
    public void Play(string name, bool loop)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);//look up array
        if(s == null){
            Debug.Log("No auido source attached to "+name);
            return;//early exit if no sound attached
        }
        s.source.Play();
        if(loop) s.source.loop = true;
    }

    public void SetVolume(string name, float target)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);//look up array
        s.source.volume = target;
    }
    
    public void SetMute(string name, bool isMute)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);//look up array
        s.source.mute = isMute;
    }
}
