using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider sfxSlider, musicSlider;
    public Image muteMusic, muteSFX;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            AudioManager.Instance.SetVolumeSFX(PlayerPrefs.GetFloat("SFXVolume"));
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            AudioManager.Instance.SetVolumeSFX(1f);
            sfxSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            AudioManager.Instance.SetVolumeMusic(PlayerPrefs.GetFloat("MusicVolume"));
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            AudioManager.Instance.SetVolumeMusic(1f);
            musicSlider.value = 1f;
        }

        //Toggle
        muteSFX.gameObject.SetActive(AudioManager.Instance.SFXAudioSource.mute);
        muteMusic.gameObject.SetActive(AudioManager.Instance.MusicAudioSource.mute);
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        muteMusic.gameObject.SetActive(AudioManager.Instance.MusicAudioSource.mute);
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        muteSFX.gameObject.SetActive(AudioManager.Instance.SFXAudioSource.mute);
    }

    public void SetSFXVolume()
    {
        AudioManager.Instance.SetVolumeSFX(sfxSlider.value);
    }

    public void SetMusicVolume()
    {
        AudioManager.Instance.SetVolumeMusic(musicSlider.value);
    }

}
