using System;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.alternatives.Add(s.clip);

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.loop = s.loop;
        }

        RestoreVolumes();
    }

    private void RestoreVolumes()
    {
        float sfxSavedPrefs = PlayerPrefs.GetFloat(SoundMenu.PREFS_SFX, SoundManager.instance.GetVolumeForType(SoundType.SFX));
        float songSavedPrefs = PlayerPrefs.GetFloat(SoundMenu.PREFS_SONG, SoundManager.instance.GetVolumeForType(SoundType.SONG));

        SetVolumeForType(SoundType.SFX, sfxSavedPrefs);
        SetVolumeForType(SoundType.SONG, songSavedPrefs);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }
        s.source.Play();
    }

    public void PlayOrAlternative(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        if (s.alternatives != null && s.alternatives.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, s.alternatives.Count);
            Sound copy = s;
            copy.source.clip = s.alternatives[randomIndex];
            copy.source.Play();
            copy = null;
        }
        else
        {
            s.source.Play();
        }
    }

    public void PlayReversed(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }

        s.source.timeSamples = s.source.clip.samples - 1;
        s.source.pitch = -1;

        s.source.Play();
    }

    public void PlayWithPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }
        s.pitch = pitch;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found");
            return;
        }
        s.source.Stop();
    }

    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void SetVolumeForType(SoundType soundType, float volume)
    {
        Sound[] soundsOfType = Array.FindAll(sounds, sound => sound.soundType == soundType && sound.source != null);
        if (soundsOfType == null || soundsOfType.Length <= 0)
        {
            Debug.LogWarning("Sounds of type " + soundType.ToString() + " not found");
            return;
        }

        foreach (Sound s in soundsOfType)
        {
            s.source.volume = volume;
        }
    }

    public float GetVolumeForType(SoundType soundType)
    {
        Sound[] soundsOfType = Array.FindAll(sounds, sound => sound.soundType == soundType && sound.source != null);
        if (soundsOfType == null || soundsOfType.Length <= 0)
        {
            Debug.LogWarning("Sounds of type " + soundType.ToString() + " not found");
            return 0.4f;
        }

        soundsOfType = soundsOfType.OrderBy(x => x.volume).ToArray();

        return soundsOfType[0].volume;
    }
}