using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TextMeshProUGUI volumeText;

    public Slider volumeSlider;

    // Used to set the Volume in the options menu

    // sets the inital volume perentage seen in the options menu
    public void Start()
    {   
        float volume;
        audioMixer.GetFloat("Volume", out volume);
        float volumePercent = Mathf.Pow(10,(volume/20)) * 100f;
        volumeText.text = volumePercent.ToString();
        volumeSlider.value = volumePercent/10f;
    }
    public void SetVolume(float volume){

        float volumePercent = volume * 10f;

        if(volume == 0f){
            volume = 0.0001f;
        }else{
            volume = volume/10;
        }
        audioMixer.SetFloat("Volume", Mathf.Log10(volume)*20f);

        volumeText.text = volumePercent.ToString();

    }

    public void SetFullScreen(bool fullscreen){

        Debug.Log(fullscreen);

        Screen.fullScreen = fullscreen;
    }
}
