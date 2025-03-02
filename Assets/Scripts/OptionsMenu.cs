using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer soundFXMixer;
    public AudioMixer musicMixer;
    //public AudioMixer audioMixer;

    public TextMeshProUGUI volumeFXText;
    public TextMeshProUGUI volumeMusicText;

    public Slider volumeFXSlider;
    public Slider volumeMusicSlider;

    // Used to set the Volume in the options menu

    // sets the inital volume perentage seen in the options menu
    public void Start()
    {   
        float volumeFX;
        soundFXMixer.GetFloat("Volume", out volumeFX);
        float volumeFXPercent = Mathf.Pow(10,(volumeFX/20)) * 100f;
        volumeFXText.text = volumeFXPercent.ToString();
        volumeFXSlider.value = volumeFXPercent/10f;

        float volumeMusic;
        musicMixer.GetFloat("Volume", out volumeMusic);
        float volumeMusicPercent = Mathf.Pow(10,(volumeMusic/20)) * 100f;
        volumeMusicText.text = volumeMusicPercent.ToString();
        volumeMusicSlider.value = volumeMusicPercent/10f;
    }
    public void SetVolumeFX(float volume){

        float volumeFXPercent = volume * 10f;

        if(volume == 0f){
            volume = 0.0001f;
        }else{
            volume = volume/10;
        }
        soundFXMixer.SetFloat("Volume", Mathf.Log10(volume)*20f);

        volumeFXText.text = volumeFXPercent.ToString();

    }

    public void SetVolumeMusic(float volume){

        float volumeMusicPercent = volume * 10f;

        if(volume == 0f){
            volume = 0.0001f;
        }else{
            volume = volume/10;
        }
        musicMixer.SetFloat("Volume", Mathf.Log10(volume)*20f);

        volumeMusicText.text = volumeMusicPercent.ToString();

    }

    public void SetFullScreen(bool fullscreen){

        Debug.Log(fullscreen);

        Screen.fullScreen = fullscreen;
    }
}
