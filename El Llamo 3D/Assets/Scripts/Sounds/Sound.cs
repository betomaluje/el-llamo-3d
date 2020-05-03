using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public SoundType soundType;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    [Range(0f, 1f)]
    public float spatialBlend;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public enum SoundType
{
    SFX, SONG
}