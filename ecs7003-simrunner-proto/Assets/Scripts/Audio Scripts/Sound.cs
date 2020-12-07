using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;//name of the sound component
    public AudioClip clip;//the audio file
    [Range(0f,1f)]
    public float volume;//the volume of the sound
    [Range(0.1f,3f)]
    public float pitch;//the pitch of it

    [HideInInspector]//don't show it in inspector
    public AudioSource source;//the source for the audio file
    public bool loop;//for toggling loop
}
