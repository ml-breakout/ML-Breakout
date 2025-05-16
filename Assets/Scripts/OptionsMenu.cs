using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    // public AudioMixer musicMixer;
    //public AudioMixer audioMixer;

    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI volumeFXText;
    public TextMeshProUGUI volumeMusicText;

    public Slider MasterSlider;
    public Slider volumeFXSlider;
    public Slider volumeMusicSlider;

    // Used to set the Volume in the options menu

    // sets the inital volume perentage seen in the options menu
    public void Start()
    {   

        float Volume;
        audioMixer.GetFloat("Volume", out Volume);
        float volumePercent = Mathf.Pow(10,(Volume/20)) * 100f;
        volumeFXText.text = volumePercent.ToString();
        MasterSlider.value = volumePercent/10f;

        float SFXVolume;
        audioMixer.GetFloat("SFXVolume", out SFXVolume);
        float SFXVolumePercent = Mathf.Pow(10,(SFXVolume/20)) * 100f;
        volumeFXText.text = SFXVolumePercent.ToString();
        volumeFXSlider.value = SFXVolumePercent/10f;

        float volumeMusic;
        audioMixer.GetFloat("MusicVolume", out volumeMusic);
        float volumeMusicPercent = Mathf.Pow(10,(volumeMusic/20)) * 100f;
        volumeMusicText.text = volumeMusicPercent.ToString();
        volumeMusicSlider.value = volumeMusicPercent/10f;
    }

    public void SetMasterVolume(float volume){

        float volumePercent = volume * 10f;

        if(volume == 0f){
            volume = 0.0001f;
        }else{
            volume = volume/10;
        }
        audioMixer.SetFloat("Volume", Mathf.Log10(volume)*20f);

        masterVolumeText.text = volumePercent.ToString();

    }
    public void SetSFXVolume(float volume){

        float volumePercent = volume * 10f;

        if(volume == 0f){
            volume = 0.0001f;
        }else{
            volume = volume/10;
        }
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume)*20f);

        volumeFXText.text = volumePercent.ToString();

    }

    public void SetMusicVolume(float volume){

        float volumePercent = volume * 10f;

        if(volume == 0f){
            volume = 0.0001f;
        }else{
            volume = volume/10;
        }
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume)*20f);

        volumeMusicText.text = volumePercent.ToString();

    }

    public void SetFullScreen(bool fullscreen){

        Debug.Log(fullscreen);

        Screen.fullScreen = fullscreen;
    }
}
