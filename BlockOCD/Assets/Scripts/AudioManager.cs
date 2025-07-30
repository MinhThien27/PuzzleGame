using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX settings")]
    public AudioSource SFXAudioSource;
    public AudioClip[] SFXClips;

    [Header("Background music settings")]
    public AudioSource MusicAudioSource;
    //public AudioClip[] MusicClips;
    public AudioClip Music;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //Setup music
        MusicAudioSource.clip = Music;
        MusicAudioSource.loop = true;
        
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SetVolumeSFX(PlayerPrefs.GetFloat("SFXVolume"));
        }
        else
        {
            SFXAudioSource.volume = 1f;
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            SetVolumeMusic(PlayerPrefs.GetFloat("MusicVolume"));
        }
        else
        {
            MusicAudioSource.volume = 1f;
        }
        MusicAudioSource.Play();
    }

    public void ToggleMusic()
    {
        MusicAudioSource.mute = !MusicAudioSource.mute;
    }

    public void ToggleSFX()
    {
        SFXAudioSource.mute = !SFXAudioSource.mute;
    }

    public void PlaySFX(string name)
    {
        AudioClip sfx = System.Array.Find(SFXClips, clip => clip.name == name);
        if(sfx != null)
        {
            SFXAudioSource.PlayOneShot(sfx);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }

    public void SetVolumeSFX(float volume)
    {
        SFXAudioSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeMusic(float volume)
    {
        MusicAudioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        SFXAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume");
        MusicAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume");
    }
}
